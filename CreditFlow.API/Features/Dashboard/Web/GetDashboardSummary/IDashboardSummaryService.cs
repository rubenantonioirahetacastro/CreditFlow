namespace CreditFlow.API.Features.Dashboard.Web.GetDashboardSummary;

public interface IDashboardSummaryService
{
    Task<DashboardSummaryResponse> GetSummaryAsync();
}
