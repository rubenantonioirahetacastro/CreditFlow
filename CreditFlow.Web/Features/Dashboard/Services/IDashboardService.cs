using CreditFlow.Web.Core.Http;
using CreditFlow.Web.Features.Dashboard.Models;

namespace CreditFlow.Web.Features.Dashboard.Services;

public interface IDashboardService
{
    Task<ApiResult<DashboardResumenResponse>> ObtenerResumenAsync();
}
