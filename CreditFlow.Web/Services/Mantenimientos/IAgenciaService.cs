using CreditFlow.Web.Models.Mantenimientos;

namespace CreditFlow.Web.Services.Mantenimientos;

public interface IAgenciaService
{
    Task<List<AgenciaDto>> ObtenerTodasAsync();

    Task<(bool Exito, string? Mensaje)> CrearAsync(CrearAgenciaRequest request);

    Task<(bool Exito, string? Mensaje)> ActualizarAsync(int id, ActualizarAgenciaRequest request);

    Task<(bool Exito, string? Mensaje)> EliminarAsync(int id);
}
