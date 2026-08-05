using CrediAvanzaAPI.Domain.Entities;

namespace CrediAvanzaAPI.Application.Interfaces
{
    public interface ISegmentoUsuraService
    {
        Task ValidarTasaAsync(decimal montoCredito, List<CredCalendario> calendario, DateTime fechaCredito);
    }
}
