using CrediAvanzaAPI.Models;
using CrediAvanzaAPI.Application.Requests;

namespace CrediAvanzaAPI.Application.Interfaces
{
    public interface IPagoService
    {
        Task<List<CredCalendario>> RegistrarPagoAsync(PagoRequest request);
    }
}