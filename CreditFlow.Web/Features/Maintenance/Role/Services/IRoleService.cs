using CreditFlow.Web.Core.Http;
using CreditFlow.Web.Core.Security;
using CreditFlow.Web.Features.Maintenance.Role.Models;

namespace CreditFlow.Web.Features.Maintenance.Role.Services;

public interface IRoleService
{
    Task<ApiResult<List<RoleDto>>> ObtenerTodosAsync();

    Task<(bool Exito, string? Mensaje)> CrearAsync(CrearRoleRequest request);

    Task<(bool Exito, string? Mensaje)> ActualizarAsync(int id, ActualizarRoleRequest request);

    /// <summary>Permisos de menú configurados para el rol.</summary>
    Task<ApiResult<List<MenuPermission>>> ObtenerPermisosAsync(int idRol);

    /// <summary>Reemplaza todos los permisos de menú del rol.</summary>
    Task<(bool Exito, string? Mensaje)> GuardarPermisosAsync(int idRol, IReadOnlyCollection<MenuPermission> permisos);
}
