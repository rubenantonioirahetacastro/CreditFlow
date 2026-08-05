using CrediAvanzaAPI.Application.Requests;

namespace CrediAvanzaAPI.Application.Interfaces
{
    public interface IGastoService
    {
        Task<decimal> ObtenerGastoAsync(CreditoRequest request);
    }
}
