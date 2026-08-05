using CreditFlow.API.Domain.Entities;

namespace CreditFlow.API.Application.Interfaces
{
    public interface ISegmentoUsuraService
    {
        Task ValidarTasaAsync(decimal montoCredito, List<CredCalendario> calendario, DateTime fechaCredito);
    }
}
