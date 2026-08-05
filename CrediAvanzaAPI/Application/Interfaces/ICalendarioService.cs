using System.Collections.Generic;
using System.Threading.Tasks;
using CrediAvanzaAPI.Domain.Entities;

namespace CrediAvanzaAPI.Application.Interfaces
{
    public interface ICalendarioService
    {
        Task<List<CredCalendario>> GenerarCalendarioAsync(int nCodAge, int nCodCred);
        Task<List<CredCalendario>> ProyectarCalendarioAsync(decimal nCapital, int nPlazo, int nSubProd, int nCodAge);
    }
}
