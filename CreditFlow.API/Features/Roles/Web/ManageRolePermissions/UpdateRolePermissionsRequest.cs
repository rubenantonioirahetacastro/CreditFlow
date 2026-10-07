using CreditFlow.API.Features.Roles.Shared.Permissions;

namespace CreditFlow.API.Features.Roles.Web.ManageRolePermissions;

/// <summary>Reemplaza todos los permisos del rol por esta lista (las opciones que no vengan quedan sin acceso).</summary>
public class UpdateRolePermissionsRequest
{
    public List<RolePermissionDto> Permisos { get; set; } = [];
}
