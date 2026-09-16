using CreditFlow.Web.Features.Authentication.Services;

namespace CreditFlow.Web.Features.Authentication;

public static class AuthenticationFeatureRegistration
{
    public static IServiceCollection AddAuthenticationFeature(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthApiService>();
        return services;
    }
}
