using CreditFlow.Web.Models.Mantenimientos;

namespace CreditFlow.Web.Services.Mantenimientos;

public interface ILineaCreditoService
{
    Task<List<LineaCreditoDto>> ObtenerTodasAsync();

    Task<(bool Exito, string? Mensaje)> CrearAsync(CrearLineaCreditoRequest request);

    Task<(bool Exito, string? Mensaje)> ActualizarAsync(int id, ActualizarLineaCreditoRequest request);

    Task<(bool Exito, string? Mensaje)> EliminarAsync(int id);
}
