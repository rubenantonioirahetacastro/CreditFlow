using CreditFlow.API.Features.Mantenimientos.Empleados.DTOs;
using CreditFlow.API.Features.Mantenimientos.Empleados.Requests;

namespace CreditFlow.API.Features.Mantenimientos.Empleados.Services
{
    public interface IEmpleadoService
    {
        Task<List<EmpleadoDto>> ObtenerTodosAsync();

        Task<EmpleadoDto> CrearAsync(CrearEmpleadoRequest request);

        Task<EmpleadoDto?> ActualizarAsync(int id, ActualizarEmpleadoRequest request);

        Task<bool> EliminarAsync(int id);

        Task<(Stream Stream, string ContentType)?> ObtenerFotoAsync(int id);
    }
}
