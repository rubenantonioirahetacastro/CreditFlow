using CreditFlow.API.Application.DTOs.Mantenimientos;
using CreditFlow.API.Application.Requests.Mantenimientos;

namespace CreditFlow.API.Application.Interfaces.Mantenimientos
{
    public interface IAgenciaService
    {
        Task<List<AgenciaDto>> ObtenerTodasAsync();

        Task<AgenciaDto?> ObtenerPorIdAsync(int id);

        Task<AgenciaDto> CrearAsync(CrearAgenciaRequest request);

        Task<AgenciaDto?> ActualizarAsync(int id, ActualizarAgenciaRequest request);

        Task<bool> EliminarAsync(int id);
    }
}
