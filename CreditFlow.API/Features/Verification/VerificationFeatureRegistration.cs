using CreditFlow.API.Features.Verification.Mobile.GetClients;
using CreditFlow.API.Features.Verification.Web.GetCreditRequests;

namespace CreditFlow.API.Features.Verification;

public static class VerificationFeatureRegistration
{
    public static IServiceCollection AddVerificationFeature(this IServiceCollection services)
    {
        services.AddScoped<
            IObtenerClientListVerifierHandler,
            ObtenerClientListVerifierHandler>();
        services.AddScoped<
            IObtenerBandejaVerificacionHandler,
            ObtenerBandejaVerificacionHandler>();

        return services;
    }
}
