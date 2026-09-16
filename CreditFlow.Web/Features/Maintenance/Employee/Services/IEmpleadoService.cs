using CreditFlow.Web.Core.Http;
using CreditFlow.Web.Features.Maintenance.Employee.Models;

namespace CreditFlow.Web.Features.Maintenance.Employee.Services;

public interface IEmpleadoService
{
    Task<ApiResult<List<EmpleadoDto>>> ObtenerTodosAsync();

    Task<(bool Exito, string? Mensaje)> CrearAsync(CrearEmpleadoRequest request);

    Task<(bool Exito, string? Mensaje)> ActualizarAsync(int id, ActualizarEmpleadoRequest request);

    Task<(bool Exito, string? Mensaje)> EliminarAsync(int id);

    Task<string?> ObtenerFotoDataUrlAsync(int idEmpleado);
}
