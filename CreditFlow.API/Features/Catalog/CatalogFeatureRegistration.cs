using CreditFlow.API.Features.Catalog.Shared;

namespace CreditFlow.API.Features.Catalog;

public static class CatalogFeatureRegistration
{
    public static IServiceCollection AddCatalogFeature(this IServiceCollection services)
    {
        services.AddScoped<ICatalogRepository, CatalogRepository>();
        return services;
    }
}
