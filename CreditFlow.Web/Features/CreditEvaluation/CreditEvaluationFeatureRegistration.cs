using CreditFlow.Web.Features.CreditEvaluation.Services;

namespace CreditFlow.Web.Features.CreditEvaluation;

public static class CreditEvaluationFeatureRegistration
{
    public static IServiceCollection AddCreditEvaluationFeature(this IServiceCollection services)
    {
        services.AddScoped<IEvaluacionCreditoService, EvaluacionCreditoApiService>();
        return services;
    }
}
