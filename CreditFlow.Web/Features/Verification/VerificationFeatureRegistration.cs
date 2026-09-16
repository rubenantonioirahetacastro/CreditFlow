using CreditFlow.Web.Features.Verification.Services;

namespace CreditFlow.Web.Features.Verification;

public static class VerificationFeatureRegistration
{
    public static IServiceCollection AddVerificationFeature(this IServiceCollection services)
    {
        services.AddScoped<IVerificacionService, VerificacionApiService>();
        return services;
    }
}
