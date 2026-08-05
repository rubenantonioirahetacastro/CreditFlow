using CrediAvanzaAPI.Domain.Entities;
using CrediAvanzaAPI.Application.Requests;

namespace CrediAvanzaAPI.Application.Interfaces
{
    public interface IPagoService
    {
        Task<List<CredCalendario>> RegistrarPagoAsync(PagoRequest request);
    }
}