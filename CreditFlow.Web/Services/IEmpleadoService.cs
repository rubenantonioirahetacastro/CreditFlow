using CreditFlow.Web.Models;

namespace CreditFlow.Web.Services;

public interface IEmpleadoService
{
    Task<List<EmpleadoDto>> ObtenerTodosAsync();

    Task<(bool Exito, string? Mensaje)> CrearAsync(CrearEmpleadoRequest request);
}
