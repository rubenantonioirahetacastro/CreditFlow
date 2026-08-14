using CreditFlow.API.Features.Calendario.Requests;

namespace CreditFlow.API.Features.Calendario.Services
{
    public interface IGastoService
    {
        Task<decimal> ObtenerGastoAsync(CreditoRequest request);
    }
}
