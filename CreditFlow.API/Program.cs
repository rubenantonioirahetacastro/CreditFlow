using CreditFlow.API.Core.Diagnostics;
using CreditFlow.API.Core.Errors;
using CreditFlow.API.Core.Security;
using CreditFlow.API.Core.Serialization;
using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Infrastructure.Data;
using CreditFlow.API.Core.Email;
using CreditFlow.API.Core.Storage;
using CreditFlow.API.Infrastructure.Services;
using CreditFlow.API.Infrastructure.Diagnostics;
using CreditFlow.API.Features.Credit;
using CreditFlow.API.Features.Employee;
using CreditFlow.API.Features.Verification;
using CreditFlow.API.Features.Simulator;
using CreditFlow.API.Features.Payment;
using CreditFlow.API.Features.Catalog;
using CreditFlow.API.Features.Agency;
using CreditFlow.API.Features.Roles;
using CreditFlow.API.Features.Dashboard;
using CreditFlow.API.Features.Authentication;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.ApplicationInsights.TelemetryConverters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

var applicationInsightsConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.ApplicationInsights(applicationInsightsConnectionString, new TraceTelemetryConverter())
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddCreditFeature();
builder.Services.AddAuthenticationFeature();
builder.Services.AddVerificationFeature();
builder.Services.AddSimulatorFeature();
builder.Services.AddEmployeeFeature(builder.Configuration);
builder.Services.AddPaymentFeature();
builder.Services.AddCatalogFeature();
builder.Services.AddAgencyFeature();
builder.Services.AddRoleFeature();
builder.Services.AddDashboardFeature();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddScoped<IErrorLogger, DatabaseErrorLogger>();
builder.Services.AddScoped<IBlobStorageService, AzureBlobStorageService>();

builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = applicationInsightsConnectionString;
});

builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("Default")
        ?? throw new InvalidOperationException("Connection string 'Default' not found."));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
    });
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .ToDictionary(
                entry => entry.Key,
                entry => entry.Value!.Errors
                    .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                        ? "El valor enviado no es válido."
                        : error.ErrorMessage)
                    .ToArray());

        var message = errors.Values
            .SelectMany(items => items)
            .FirstOrDefault()
            ?? "Los datos enviados no son válidos.";

        return new BadRequestObjectResult(new ApiErrorResponse(
            "validation_error",
            message,
            errors));
    };
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure JWT authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "ChangeThisSecretInProduction_ReplaceMeWithStrongKey";
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero,
            NameClaimType = ClaimTypes.Name
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        AuthorizationPolicies.MobileVerifier,
        policy => policy
            .RequireAuthenticatedUser()
            .RequireAssertion(context =>
            {
                return RoleAuthorization.HasAnyRoleId(
                    context.User,
                    RoleCapabilities.VerificationAccess);
            }));

    options.AddPolicy(
        AuthorizationPolicies.Maintenance,
        policy => policy
            .RequireAuthenticatedUser()
            .RequireAssertion(context => RoleAuthorization.HasAnyRoleId(
                context.User,
                RoleIds.MaintenanceAccess)));

    options.AddPolicy(
        AuthorizationPolicies.Administration,
        policy => policy
            .RequireAuthenticatedUser()
            .RequireAssertion(context => RoleAuthorization.HasAnyRoleId(
                context.User,
                RoleIds.AdministrationAccess)));

    options.AddPolicy(
        AuthorizationPolicies.CalendarConfiguration,
        policy => policy
            .RequireAuthenticatedUser()
            .RequireAssertion(context => RoleAuthorization.HasAnyRoleId(
                context.User,
                RoleIds.CalendarConfigurationAccess)));
});

// Swagger with Bearer token support
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement{
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference{
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            }, new string[] { }
        }
    });
});

builder.Services.AddDbContext<DbNegocioContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("conexion")
    ?? throw new InvalidOperationException("Connection string 'API_CrediAvanzaContext' not found.")));
var app = builder.Build();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    });
//}

   if (!app.Environment.IsDevelopment())
    {
        app.UseHttpsRedirection();
    }

app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

// deploy inicial


//Scaffold - DbContext "Server=DESKTOP-KTHL7K7\SQLEXPRESS;Database=DbNegocio;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer - OutputDir Models - Context DbNegocioContext - Force
//Server=tcp:crediavanza.database.windows.net,1433;Initial Catalog=DbNegocio;Persist Security Info=False;User ID=crediavanza;Password=Pepe1234;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
//Scaffold-DbContext "Server=tcp:crediavanza.database.windows.net,1433;Initial Catalog=DbNegocio;Persist Security Info=False;User ID=crediavanza;Password=Pepe1234;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -Context DbNegocioContext -Force
