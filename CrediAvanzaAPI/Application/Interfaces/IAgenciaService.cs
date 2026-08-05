using System.Collections.Generic;
using System.Threading.Tasks;
using CrediAvanzaAPI.Domain.Entities;

namespace CrediAvanzaAPI.Application.Interfaces
{
    public interface IAgenciaService
    {
        Task<List<Agencia>> AllAgencias();
        Task<Agencia?> GetAgenciaById(int id);
        Task<bool> AddAgencia(Agencia agencia);
        Task<bool> UpdateAgencia(Agencia agencia);
        Task<bool> DeleteAgencia(int id);
    }
}
