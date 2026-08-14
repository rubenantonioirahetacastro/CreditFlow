using CreditFlow.Web.Features.Mantenimientos.LineasCredito.Models;

namespace CreditFlow.Web.Features.Mantenimientos.LineasCredito.Services;

public interface ILineaCreditoService
{
    Task<List<LineaCreditoDto>> ObtenerTodasAsync();

    Task<(bool Exito, string? Mensaje)> CrearAsync(CrearLineaCreditoRequest request);

    Task<(bool Exito, string? Mensaje)> ActualizarAsync(int id, ActualizarLineaCreditoRequest request);

    Task<(bool Exito, string? Mensaje)> EliminarAsync(int id);
}
