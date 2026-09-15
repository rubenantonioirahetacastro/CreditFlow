using CreditFlow.API.Core.Security;
using CreditFlow.API.Core.Errors;
using CreditFlow.API.Features.Roles.Shared.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CreditFlow.API.Features.Roles.Web.ManageRoles
{
    [Route("api/mantenimientos/roles")]
    [ApiController]
    public class ManageRolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public ManageRolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ObtenerActivos()
        {
            var roles = await _roleService.GetActiveAsync();
            return Ok(roles);
        }
        
        [HttpGet("todos")]
        [Authorize(Policy = AuthorizationPolicies.Maintenance)]
        public async Task<IActionResult> ObtenerTodos()
        {
            var roles = await _roleService.GetAllAsync();
            return Ok(roles);
        }

        [HttpPost]
        [Authorize(Policy = AuthorizationPolicies.Maintenance)]
        public async Task<IActionResult> Crear([FromBody] CreateRoleRequest request)
        {
            var role = await _roleService.CreateAsync(request);
            return Created($"api/mantenimientos/roles/{role.IdRol}", role);
        }
        
        [HttpPut("{id}")]
        [Authorize(Policy = AuthorizationPolicies.Maintenance)]
        public async Task<IActionResult> Actualizar(int id, [FromBody] UpdateRoleRequest request)
        {
            var role = await _roleService.UpdateAsync(id, request);
            if (role == null)
                throw new ResourceNotFoundException(RoleErrors.NotFound);

            return Ok(role);
        }
    }
}
