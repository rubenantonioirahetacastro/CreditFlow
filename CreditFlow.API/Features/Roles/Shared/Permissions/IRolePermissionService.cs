namespace CreditFlow.API.Features.Roles.Shared.Permissions;

public interface IRolePermissionService
{
    /// <summary>Permisos configurados para un rol; null si el rol no existe.</summary>
    Task<List<RolePermissionDto>?> GetByRoleAsync(int roleId);

    /// <summary>Reemplaza los permisos del rol por la lista indicada; false si el rol no existe.</summary>
    Task<bool> ReplaceAsync(int roleId, IReadOnlyCollection<RolePermissionDto> permissions);

    /// <summary>Unión de los permisos de varios roles (los del usuario autenticado).</summary>
    Task<List<RolePermissionDto>> GetForRolesAsync(IReadOnlyCollection<int> roleIds);
}
