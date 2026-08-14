using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Features.Pago.Requests;

namespace CreditFlow.API.Features.Pago.Services
{
    public interface IPagoService
    {
        Task<List<CredCalendario>> RegistrarPagoAsync(PagoRequest request);
    }
}