using CreditFlow.Web.Features.Dashboard.Services;

namespace CreditFlow.Web.Features.Dashboard;

public static class DashboardFeatureRegistration
{
    public static IServiceCollection AddDashboardFeature(this IServiceCollection services)
    {
        services.AddScoped<IDashboardService, DashboardApiService>();
        return services;
    }
}
