using CreditFlow.API.Features.Authentication.Mobile.EmployeeLogin;
using CreditFlow.API.Features.Authentication.Shared.Session;
using CreditFlow.API.Features.Authentication.Shared.Token;

namespace CreditFlow.API.Features.Authentication;

public static class AuthenticationFeatureRegistration
{
    public static IServiceCollection AddAuthenticationFeature(this IServiceCollection services)
    {
        services.AddScoped<IUserSessionService, UserSessionService>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddHttpContextAccessor();
        services.AddScoped<EmployeeLoginHandler>();
        return services;
    }
}
