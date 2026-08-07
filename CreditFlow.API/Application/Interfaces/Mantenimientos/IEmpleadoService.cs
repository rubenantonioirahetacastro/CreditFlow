using CreditFlow.API.Application.DTOs.Mantenimientos;
using CreditFlow.API.Application.Requests.Mantenimientos;

namespace CreditFlow.API.Application.Interfaces.Mantenimientos
{
    public interface IEmpleadoService
    {
        Task<List<EmpleadoDto>> ObtenerTodosAsync();

        Task<EmpleadoDto> CrearAsync(CrearEmpleadoRequest request);
    }
}
