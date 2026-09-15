using CreditFlow.API.Features.Agency.Web.ManageAgencies;

namespace CreditFlow.API.Features.Agency;

public static class AgencyFeatureRegistration
{
    public static IServiceCollection AddAgencyFeature(this IServiceCollection services)
    {
        services.AddScoped<IAgencyService, AgencyService>();
        return services;
    }
}
