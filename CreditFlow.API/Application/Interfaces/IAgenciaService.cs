using System.Collections.Generic;
using System.Threading.Tasks;
using CreditFlow.API.Domain.Entities;

namespace CreditFlow.API.Application.Interfaces
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
