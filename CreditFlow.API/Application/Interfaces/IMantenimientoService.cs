using CreditFlow.API.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CreditFlow.API.Application.Interfaces
{
    public interface IMantenimientoService
    {
        Task<IEnumerable<Mantenimiento>> GetAllAsync();
        Task<Mantenimiento?> GetByIdAsync(int id);
        Task<Mantenimiento> CreateAsync(Mantenimiento item);
        Task<bool> UpdateAsync(int id, Mantenimiento item);
        Task<bool> DeleteAsync(int id);
    }
}
