using CreditFlow.API.Application.Interfaces.Mantenimientos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CreditFlow.API.Controllers.Mantenimientos
{
    [Route("api/mantenimientos/roles")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ObtenerActivos()
        {
            var roles = await _roleService.ObtenerActivosAsync();
            return Ok(roles);
        }
    }
}
