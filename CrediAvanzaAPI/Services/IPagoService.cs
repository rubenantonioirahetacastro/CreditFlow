using CrediAvanzaAPI.Models;
using CrediAvanzaAPI.Application.Requests;

namespace CrediAvanzaAPI.Services
{
    public interface IPagoService
    {
        Task<List<CredCalendario>> RegistrarPagoAsync(PagoRequest request);
    }
}