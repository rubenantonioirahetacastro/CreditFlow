using CreditFlow.API.Core.Errors;
using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Features.Roles.Shared.Errors;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Roles.Shared.Permissions;

public sealed class RolePermissionService(DbNegocioContext context) : IRolePermissionService
{
    public async Task<List<RolePermissionDto>?> GetByRoleAsync(int roleId)
    {
        if (!await context.Roles.AnyAsync(r => r.IdRol == roleId))
            return null;

        var permissions = await context.RolMenuPermisos
            .AsNoTracking()
            .Where(p => p.IdRol == roleId)
            .OrderBy(p => p.CClaveMenu)
            .Select(p => new RolePermissionDto
            {
                Clave = p.CClaveMenu,
                Ver = p.BVer,
                Crear = p.BCrear,
                Editar = p.BEditar,
                Eliminar = p.BEliminar
            })
            .ToListAsync();

        return EnsureSafeguard(permissions, [roleId]);
    }

    public async Task<bool> ReplaceAsync(int roleId, IReadOnlyCollection<RolePermissionDto> permissions)
    {
        if (!await context.Roles.AnyAsync(r => r.IdRol == roleId))
            return false;

        foreach (var permission in permissions)
        {
            if (!RolePermissionRules.IsValidKey(permission.Clave))
                throw new RequestValidationException(RoleErrors.InvalidMenuKey(permission.Clave));
        }

        if (permissions.Select(p => p.Clave).Distinct().Count() != permissions.Count)
            throw new RequestValidationException(RoleErrors.DuplicatedMenuKey);

        // Solo se guardan las opciones con algún permiso; crear/editar/eliminar implican ver.
        var toSave = EnsureSafeguard(permissions.ToList(), [roleId])
            .Where(p => p.Ver || p.Crear || p.Editar || p.Eliminar)
            .Select(p => new RolMenuPermiso
            {
                IdRol = roleId,
                CClaveMenu = p.Clave,
                BVer = true,
                BCrear = p.Crear,
                BEditar = p.Editar,
                BEliminar = p.Eliminar,
                DFechaModificacion = DateTime.UtcNow
            })
            .ToList();

        await using var transaction = await context.Database.BeginTransactionAsync();
        await context.RolMenuPermisos.Where(p => p.IdRol == roleId).ExecuteDeleteAsync();
        await context.RolMenuPermisos.AddRangeAsync(toSave);
        await context.SaveChangesAsync();
        await transaction.CommitAsync();
        return true;
    }

    public async Task<List<RolePermissionDto>> GetForRolesAsync(IReadOnlyCollection<int> roleIds)
    {
        var rows = await context.RolMenuPermisos
            .AsNoTracking()
            .Where(p => roleIds.Contains(p.IdRol) && p.IdRolNavigation.Activo)
            .ToListAsync();

        // Un usuario con varios roles recibe la suma de sus permisos.
        var union = rows
            .GroupBy(p => p.CClaveMenu)
            .Select(g => new RolePermissionDto
            {
                Clave = g.Key,
                Ver = g.Any(p => p.BVer),
                Crear = g.Any(p => p.BCrear),
                Editar = g.Any(p => p.BEditar),
                Eliminar = g.Any(p => p.BEliminar)
            })
            .OrderBy(p => p.Clave)
            .ToList();

        return EnsureSafeguard(union, roleIds);
    }

    private static List<RolePermissionDto> EnsureSafeguard(List<RolePermissionDto> permissions, IEnumerable<int> roleIds)
    {
        if (!roleIds.Any(RolePermissionRules.AlwaysManagesPermissions))
            return permissions;

        permissions.RemoveAll(p => p.Clave == RolePermissionRules.RolePermissionsMenuKey);
        permissions.Add(RolePermissionRules.FullAccess(RolePermissionRules.RolePermissionsMenuKey));
        return permissions;
    }
}
