using CreditFlow.Web.Core.Http;
using CreditFlow.Web.Models;

namespace CreditFlow.Web.Services;

public sealed class DashboardApiService(IApiClient apiClient) : IDashboardService
{
    public async Task<DashboardResumenResponse?> ObtenerResumenAsync()
    {
        var result = await apiClient.GetAsync<DashboardResumenResponse>(
            "api/Dashboard/resumen",
            "No se pudo cargar el resumen.");
        return result.Data;
    }
}
