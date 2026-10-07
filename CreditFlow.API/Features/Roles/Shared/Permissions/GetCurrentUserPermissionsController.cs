using CreditFlow.API.Core.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CreditFlow.API.Features.Roles.Shared.Permissions;

/// <summary>
/// Permisos de menú del usuario autenticado (suma de sus roles), para armar el menú de la Web.
/// Los roles se toman del token; el endpoint no recibe identificadores.
/// </summary>
[Route("api/roles/mis-permisos")]
[ApiController]
public class GetCurrentUserPermissionsController(IRolePermissionService permissionService) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Obtener()
    {
        var roleIds = User.FindAll(CustomClaimTypes.RoleId)
            .Select(claim => int.TryParse(claim.Value, out var id) ? id : 0)
            .Where(id => id > 0)
            .Distinct()
            .ToList();

        return Ok(await permissionService.GetForRolesAsync(roleIds));
    }
}
