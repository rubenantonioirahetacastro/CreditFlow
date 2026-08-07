using CreditFlow.API.Application.DTOs.Mantenimientos;
using CreditFlow.API.Application.Interfaces.Mantenimientos;
using CreditFlow.API.Application.Requests.Mantenimientos;
using CreditFlow.API.Domain.Entities;
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

        public async Task<RoleDto> CrearAsync(CreateRoleRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Nombre))
                throw new ArgumentException("Nombre es requerido.");

            var nombre = request.Nombre.Trim();

            var existeActivo = await _context.Roles
                .AnyAsync(r => r.Activo && r.Nombre.ToLower() == nombre.ToLower());

            if (existeActivo)
                throw new InvalidOperationException($"Ya existe un rol activo con el nombre '{nombre}'.");

            var role = new Role
            {
                Nombre = nombre,
                Descripcion = request.Descripcion,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();

            return new RoleDto { IdRol = role.IdRol, Nombre = role.Nombre };
        }

        public async Task<RoleDto?> ActualizarAsync(int idRol, UpdateRoleRequest request)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.IdRol == idRol);
            if (role == null)
                return null;

            if (string.IsNullOrWhiteSpace(request.Nombre))
                throw new ArgumentException("Nombre es requerido.");

            var nombre = request.Nombre.Trim();

            var colisiona = await _context.Roles
                .AnyAsync(r => r.Activo && r.IdRol != idRol && r.Nombre.ToLower() == nombre.ToLower());

            if (colisiona)
                throw new InvalidOperationException($"Ya existe un rol activo con el nombre '{nombre}'.");

            role.Nombre = nombre;
            role.Descripcion = request.Descripcion;
            role.Activo = request.Activo;

            await _context.SaveChangesAsync();

            return new RoleDto { IdRol = role.IdRol, Nombre = role.Nombre };
        }
    }
}
