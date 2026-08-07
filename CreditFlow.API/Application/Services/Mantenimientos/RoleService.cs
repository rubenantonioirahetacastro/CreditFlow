using CreditFlow.API.Application.DTOs.Mantenimientos;
using CreditFlow.API.Application.Interfaces.Mantenimientos;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Application.Services.Mantenimientos
{
    public class RoleService : IRoleService
    {
        private readonly DbNegocioContext _context;

        public RoleService(DbNegocioContext context)
        {
            _context = context;
        }

        public async Task<List<RoleDto>> ObtenerActivosAsync()
        {
            return await _context.Roles
                .Where(r => r.Activo)
                .OrderBy(r => r.Nombre)
                .Select(r => new RoleDto
                {
                    IdRol = r.IdRol,
                    Nombre = r.Nombre
                })
                .ToListAsync();
        }
    }
}
