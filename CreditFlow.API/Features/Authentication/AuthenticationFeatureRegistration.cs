using CreditFlow.API.Features.Authentication.Shared.Session;
using CreditFlow.API.Features.Authentication.Shared.Token;

namespace CreditFlow.API.Features.Authentication;

public static class AuthenticationFeatureRegistration
{
    public static IServiceCollection AddAuthenticationFeature(this IServiceCollection services)
    {
        services.AddScoped<IUserSessionService, UserSessionService>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        return services;
    }
}
