using CreditFlow.API.Features.Mantenimientos.LineasCredito.DTOs;
using CreditFlow.API.Features.Mantenimientos.LineasCredito.Requests;

namespace CreditFlow.API.Features.Mantenimientos.LineasCredito.Services
{
    // "Admin" para no chocar con ILineaCreditoService (mismo namespace), que resuelve
    // la línea aplicable a un subproducto/monto durante el otorgamiento.
    public interface ILineaCreditoAdminService
    {
        Task<List<LineaCreditoDto>> ObtenerTodasAsync();

        Task<LineaCreditoDto?> ObtenerPorIdAsync(int id);

        Task<LineaCreditoDto> CrearAsync(CrearLineaCreditoRequest request, string? usuario);

        Task<LineaCreditoDto?> ActualizarAsync(int id, ActualizarLineaCreditoRequest request, string? usuario);

        Task<bool> EliminarAsync(int id);
    }
}
