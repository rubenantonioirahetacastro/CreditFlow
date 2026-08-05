using CrediAvanzaAPI.Application.Requests;

namespace CrediAvanzaAPI.Services
{
    public interface IGastoService
    {
        Task<decimal> ObtenerGastoAsync(CreditoRequest request);
    }
}
