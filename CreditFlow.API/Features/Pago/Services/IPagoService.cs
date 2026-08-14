using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Application.Requests;

namespace CreditFlow.API.Application.Interfaces
{
    public interface IPagoService
    {
        Task<List<CredCalendario>> RegistrarPagoAsync(PagoRequest request);
    }
}