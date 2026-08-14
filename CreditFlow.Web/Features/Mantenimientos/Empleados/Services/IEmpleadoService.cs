using CreditFlow.Web.Models.Mantenimientos;

namespace CreditFlow.Web.Services.Mantenimientos;

public interface IEmpleadoService
{
    Task<List<EmpleadoDto>> ObtenerTodosAsync();

    Task<(bool Exito, string? Mensaje)> CrearAsync(CrearEmpleadoRequest request);

    Task<(bool Exito, string? Mensaje)> ActualizarAsync(int id, ActualizarEmpleadoRequest request);

    Task<(bool Exito, string? Mensaje)> EliminarAsync(int id);

    Task<string?> ObtenerFotoDataUrlAsync(int idEmpleado);
}
