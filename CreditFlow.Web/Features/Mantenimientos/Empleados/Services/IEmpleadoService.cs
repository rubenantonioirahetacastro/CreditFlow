using CreditFlow.Web.Features.Mantenimientos.Empleados.Models;

namespace CreditFlow.Web.Features.Mantenimientos.Empleados.Services;

public interface IEmpleadoService
{
    Task<List<EmpleadoDto>> ObtenerTodosAsync();

    Task<(bool Exito, string? Mensaje)> CrearAsync(CrearEmpleadoRequest request);

    Task<(bool Exito, string? Mensaje)> ActualizarAsync(int id, ActualizarEmpleadoRequest request);

    Task<(bool Exito, string? Mensaje)> EliminarAsync(int id);

    Task<string?> ObtenerFotoDataUrlAsync(int idEmpleado);
}
