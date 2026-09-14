using CreditFlow.API.Application.DTOs;

namespace CreditFlow.API.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardResumenResponse> ObtenerResumenAsync();
}
