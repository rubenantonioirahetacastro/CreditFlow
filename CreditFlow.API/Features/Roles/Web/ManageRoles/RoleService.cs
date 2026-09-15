using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Core.Errors;
using CreditFlow.API.Features.Roles.Shared.Errors;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Roles.Web.ManageRoles
{
    public class RoleService : IRoleService
    {
        private readonly DbNegocioContext _context;

        public RoleService(DbNegocioContext context)
        {
            _context = context;
        }

        public async Task<List<RoleDto>> GetActiveAsync()
        {
            return await _context.Roles
                .Where(r => r.Activo)
                .OrderBy(r => r.Nombre)
                .Select(r => new RoleDto
                {
                    IdRol = r.IdRol,
                    Nombre = r.Nombre,
                    Descripcion = r.Descripcion,
                    Activo = r.Activo
                })
                .ToListAsync();
        }

        public async Task<List<RoleDto>> GetAllAsync()
        {
            return await _context.Roles
                .OrderBy(r => r.Nombre)
                .Select(r => new RoleDto
                {
                    IdRol = r.IdRol,
                    Nombre = r.Nombre,
                    Descripcion = r.Descripcion,
                    Activo = r.Activo
                })
                .ToListAsync();
        }

        public async Task<RoleDto> CreateAsync(CreateRoleRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Nombre))
                throw new RequestValidationException(RoleErrors.NameRequired);

            var nombre = request.Nombre.Trim();

            var existeActivo = await _context.Roles
                .AnyAsync(r => r.Activo && r.Nombre.ToLower() == nombre.ToLower());

            if (existeActivo)
                throw new ResourceConflictException(RoleErrors.ActiveNameAlreadyExists(nombre));

            var role = new Role
            {
                Nombre = nombre,
                Descripcion = request.Descripcion,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();

            return new RoleDto { IdRol = role.IdRol, Nombre = role.Nombre, Descripcion = role.Descripcion, Activo = role.Activo };
        }

        public async Task<RoleDto?> UpdateAsync(int roleId, UpdateRoleRequest request)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.IdRol == roleId);
            if (role == null)
                return null;

            if (string.IsNullOrWhiteSpace(request.Nombre))
                throw new RequestValidationException(RoleErrors.NameRequired);

            var nombre = request.Nombre.Trim();

            var colisiona = await _context.Roles
                .AnyAsync(r => r.Activo && r.IdRol != roleId && r.Nombre.ToLower() == nombre.ToLower());

            if (colisiona)
                throw new ResourceConflictException(RoleErrors.ActiveNameAlreadyExists(nombre));

            role.Nombre = nombre;
            role.Descripcion = request.Descripcion;
            role.Activo = request.Activo;

            await _context.SaveChangesAsync();

            return new RoleDto { IdRol = role.IdRol, Nombre = role.Nombre, Descripcion = role.Descripcion, Activo = role.Activo };
        }
    }
}
