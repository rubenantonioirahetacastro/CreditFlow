using CreditFlow.API.Application.Requests;

namespace CreditFlow.API.Application.Interfaces
{
    public interface IGastoService
    {
        Task<decimal> ObtenerGastoAsync(CreditoRequest request);
    }
}
