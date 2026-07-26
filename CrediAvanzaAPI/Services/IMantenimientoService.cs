using CrediAvanzaAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrediAvanzaAPI.Services
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
