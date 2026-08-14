using CreditFlow.API.Application.DTOs.Mantenimientos;
using CreditFlow.API.Application.Requests.Mantenimientos;

namespace CreditFlow.API.Application.Interfaces.Mantenimientos
{
    // "Admin" para no chocar con CreditFlow.API.Application.Services.ILineaCreditoService,
    // que resuelve la línea aplicable a un subproducto/monto durante el otorgamiento.
    public interface ILineaCreditoAdminService
    {
        Task<List<LineaCreditoDto>> ObtenerTodasAsync();

        Task<LineaCreditoDto?> ObtenerPorIdAsync(int id);

        Task<LineaCreditoDto> CrearAsync(CrearLineaCreditoRequest request, string? usuario);

        Task<LineaCreditoDto?> ActualizarAsync(int id, ActualizarLineaCreditoRequest request, string? usuario);

        Task<bool> EliminarAsync(int id);
    }
}
