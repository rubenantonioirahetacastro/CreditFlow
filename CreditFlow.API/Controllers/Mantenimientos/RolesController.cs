using CreditFlow.API.Application.Interfaces.Mantenimientos;
using CreditFlow.API.Application.Requests.Mantenimientos;
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

        [HttpPost]
        [Authorize(Roles = "Admin,Supervisor,Tecnologia")]
        public async Task<IActionResult> Crear([FromBody] CreateRoleRequest request)
        {
            try
            {
                var role = await _roleService.CrearAsync(request);
                return Created($"api/mantenimientos/roles/{role.IdRol}", role);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Mensaje = ex.Message });
            }
        }

        // No se expone DELETE físico: los roles pueden estar referenciados en
        // UsuarioRoles, y borrar la fila rompería esa integridad referencial.
        // El "borrado" de este catálogo es lógico: PUT con Activo=false. No se
        // agrega un endpoint DELETE aparte porque haría exactamente lo mismo
        // que este PUT, solo que con otro verbo — sería una ruta redundante.
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Supervisor,Tecnologia")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] UpdateRoleRequest request)
        {
            try
            {
                var role = await _roleService.ActualizarAsync(id, request);
                if (role == null)
                    return NotFound(new { Mensaje = "El rol no existe." });

                return Ok(role);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Mensaje = ex.Message });
            }
        }
    }
}
