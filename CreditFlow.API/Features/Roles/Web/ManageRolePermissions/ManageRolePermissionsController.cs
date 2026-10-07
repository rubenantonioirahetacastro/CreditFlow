using CreditFlow.API.Core.Errors;
using CreditFlow.API.Core.Security;
using CreditFlow.API.Features.Roles.Shared.Errors;
using CreditFlow.API.Features.Roles.Shared.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CreditFlow.API.Features.Roles.Web.ManageRolePermissions;

/// <summary>Mantenimiento «Permisos por rol» de la Web: qué opciones del menú ve cada rol y qué acciones puede hacer.</summary>
[Route("api/mantenimientos/roles/{id:int}/permisos")]
[ApiController]
public class ManageRolePermissionsController(IRolePermissionService permissionService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.Maintenance)]
    public async Task<IActionResult> Obtener(int id)
    {
        var permisos = await permissionService.GetByRoleAsync(id)
            ?? throw new ResourceNotFoundException(RoleErrors.NotFound);

        return Ok(permisos);
    }

    [HttpPut]
    [Authorize(Policy = AuthorizationPolicies.Administration)]
    public async Task<IActionResult> Reemplazar(int id, [FromBody] UpdateRolePermissionsRequest request)
    {
        if (!await permissionService.ReplaceAsync(id, request.Permisos))
            throw new ResourceNotFoundException(RoleErrors.NotFound);

        return NoContent();
    }
}
