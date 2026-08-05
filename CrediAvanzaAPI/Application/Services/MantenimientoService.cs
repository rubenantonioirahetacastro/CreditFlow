using CrediAvanzaAPI.Application.Interfaces;
using CrediAvanzaAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrediAvanzaAPI.Services
{
    public class MantenimientoService : IMantenimientoService
    {
        private readonly DbNegocioContext _db;

        public MantenimientoService(DbNegocioContext db)
        {
            _db = db;
        }

        public async Task<Mantenimiento> CreateAsync(Mantenimiento item)
        {
            _db.Mantenimientos.Add(item);
            await _db.SaveChangesAsync();
            return item;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var e = await _db.Mantenimientos.FindAsync(id);
            if (e == null) return false;
            _db.Mantenimientos.Remove(e);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Mantenimiento>> GetAllAsync()
        {
            return await _db.Mantenimientos.AsNoTracking().ToListAsync();
        }

        public async Task<Mantenimiento?> GetByIdAsync(int id)
        {
            return await _db.Mantenimientos.FindAsync(id);
        }

        public async Task<bool> UpdateAsync(int id, Mantenimiento item)
        {
            var exists = await _db.Mantenimientos.AnyAsync(x => x.Id == id);
            if (!exists) return false;
            item.Id = id;
            _db.Mantenimientos.Update(item);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
