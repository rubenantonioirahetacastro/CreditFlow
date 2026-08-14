using CreditFlow.Web.Features.Mantenimientos.Agencias.Models;

namespace CreditFlow.Web.Features.Mantenimientos.Agencias.Services;

public interface IAgenciaService
{
    Task<List<AgenciaDto>> ObtenerTodasAsync();

    Task<(bool Exito, string? Mensaje)> CrearAsync(CrearAgenciaRequest request);

    Task<(bool Exito, string? Mensaje)> ActualizarAsync(int id, ActualizarAgenciaRequest request);

    Task<(bool Exito, string? Mensaje)> EliminarAsync(int id);
}
