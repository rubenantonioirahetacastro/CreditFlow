using CreditFlow.Web.Core.Http;
using CreditFlow.Web.Features.Maintenance.CreditLine.Models;

namespace CreditFlow.Web.Features.Maintenance.CreditLine.Services;

public interface ILineaCreditoService
{
    Task<ApiResult<List<LineaCreditoDto>>> ObtenerTodasAsync();

    Task<(bool Exito, string? Mensaje)> CrearAsync(CrearLineaCreditoRequest request);

    Task<(bool Exito, string? Mensaje)> ActualizarAsync(int id, ActualizarLineaCreditoRequest request);

    Task<(bool Exito, string? Mensaje)> EliminarAsync(int id);
}
