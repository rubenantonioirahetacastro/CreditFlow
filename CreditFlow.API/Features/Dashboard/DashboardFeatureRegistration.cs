using CreditFlow.API.Features.Dashboard.Web.GetDashboardSummary;

namespace CreditFlow.API.Features.Dashboard;

public static class DashboardFeatureRegistration
{
    public static IServiceCollection AddDashboardFeature(this IServiceCollection services)
    {
        services.AddScoped<IDashboardSummaryService, DashboardSummaryService>();
        return services;
    }
}
