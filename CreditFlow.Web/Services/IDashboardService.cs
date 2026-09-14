using CreditFlow.Web.Models;

namespace CreditFlow.Web.Services;

public interface IDashboardService
{
    Task<DashboardResumenResponse?> ObtenerResumenAsync();
}
