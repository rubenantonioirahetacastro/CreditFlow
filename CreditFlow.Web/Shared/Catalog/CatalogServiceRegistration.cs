using CreditFlow.Web.Shared.Catalog.Services;

namespace CreditFlow.Web.Shared.Catalog;

public static class CatalogServiceRegistration
{
    public static IServiceCollection AddSharedCatalogServices(this IServiceCollection services)
    {
        services.AddScoped<ObtenerCatalogoCodigos, ObtenerCatalogoCodigosApi>();
        return services;
    }
}
