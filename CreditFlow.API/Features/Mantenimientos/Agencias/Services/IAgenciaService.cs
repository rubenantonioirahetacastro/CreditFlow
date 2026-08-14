using CreditFlow.API.Features.Mantenimientos.Agencias.DTOs;
using CreditFlow.API.Features.Mantenimientos.Agencias.Requests;

namespace CreditFlow.API.Features.Mantenimientos.Agencias.Services
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
