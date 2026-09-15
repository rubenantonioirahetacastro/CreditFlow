using System.Collections.Generic;
using System.Threading.Tasks;
using CreditFlow.API.Domain.Entities;

namespace CreditFlow.API.Features.Credit.Shared.Calendar
{
    public interface ICalendarioService
    {
        Task<List<CredCalendario>> GenerarCalendarioAsync(int nCodAge, int nCodCred);
        Task<List<CredCalendario>> ProyectarCalendarioAsync(decimal nCapital, int nPlazo, int nSubProd, int nCodAge);
    }
}
