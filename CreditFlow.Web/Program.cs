using System.Globalization;
using CreditFlow.Web.Components;
using CreditFlow.Web.Core.Http;
using CreditFlow.Web.Core.Navigation;
using CreditFlow.Web.Core.Security;
using CreditFlow.Web.Core.UI;
using CreditFlow.Web.Features.Maintenance;
using CreditFlow.Web.Features.Authentication;
using CreditFlow.Web.Features.Authentication.Endpoints;
using CreditFlow.Web.Features.CreditEvaluation;
using CreditFlow.Web.Features.Dashboard;
using CreditFlow.Web.Features.Simulator;
using CreditFlow.Web.Features.Verification;
using CreditFlow.Web.Shared.Catalog;
using Microsoft.AspNetCore.Authentication.Cookies;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var apiBaseUrl = builder.Configuration["CreditFlowApi:BaseUrl"]
    ?? throw new InvalidOperationException("Configuración 'CreditFlowApi:BaseUrl' no encontrada.");

builder.Services.AddHttpClient("CreditFlowApi", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/login";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        AuthorizationPolicies.Maintenance,
        policy => policy
            .RequireAuthenticatedUser()
            .RequireAssertion(context => RoleAuthorization.HasAnyRoleId(
                context.User,
                RoleIds.MaintenanceAccess)));

    options.AddPolicy(
        AuthorizationPolicies.CalendarConfiguration,
        policy => policy
            .RequireAuthenticatedUser()
            .RequireAssertion(context => RoleAuthorization.HasAnyRoleId(
                context.User,
                RoleIds.CalendarConfigurationAccess)));
});
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<IApiClient, ApiClient>();
builder.Services.AddScoped<PageHeaderService>();
builder.Services.AddScoped<MenuPermissionService>();
builder.Services
    .AddCoreUi()
    .AddMaintenanceFeature()
    .AddAuthenticationFeature()
    .AddCreditEvaluationFeature()
    .AddDashboardFeature()
    .AddSimulatorFeature()
    .AddVerificationFeature()
    .AddSharedCatalogServices();

builder.Services.AddRadzenComponents();

// Cultura única de la aplicación (El Salvador): moneda «$», fechas dd/MM/yyyy y punto decimal.
// Sin esto, el formato «C» y los gráficos dependen de la configuración regional del servidor.
var culturaApp = CultureInfo.GetCultureInfo("es-SV");
CultureInfo.DefaultThreadCurrentCulture = culturaApp;
CultureInfo.DefaultThreadCurrentUICulture = culturaApp;

var app = builder.Build();

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(culturaApp),
    SupportedCultures = [culturaApp],
    SupportedUICultures = [culturaApp]
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapAuthEndpoints();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
