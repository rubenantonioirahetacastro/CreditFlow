using CreditFlow.Web.Core.Http;
using CreditFlow.Web.Features.Dashboard.Models;

namespace CreditFlow.Web.Features.Dashboard.Services;

public sealed class DashboardApiService(IApiClient apiClient) : IDashboardService
{
    public Task<ApiResult<DashboardResumenResponse>> ObtenerResumenAsync() =>
        apiClient.GetAsync<DashboardResumenResponse>(
            "api/Dashboard/resumen",
            "No se pudo cargar el resumen.");
}
