using CreditFlow.API.Application.Interfaces.Mantenimientos;
using CreditFlow.API.Application.Requests.Mantenimientos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CreditFlow.API.Controllers.Mantenimientos
{
    [Route("api/mantenimientos/empleados")]
    [ApiController]
    public class EmpleadosController : ControllerBase
    {
        private readonly IEmpleadoService _empleadoService;

        public EmpleadosController(IEmpleadoService empleadoService)
        {
            _empleadoService = empleadoService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Supervisor,Tecnologia")]
        public async Task<IActionResult> ObtenerTodos()
        {
            var empleados = await _empleadoService.ObtenerTodosAsync();
            return Ok(empleados);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Crear([FromBody] CrearEmpleadoRequest request)
        {
            try
            {
                var empleado = await _empleadoService.CrearAsync(request);
                return Created($"api/mantenimientos/empleados/{empleado.IdEmpleado}", empleado);
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

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarEmpleadoRequest request)
        {
            try
            {
                var empleado = await _empleadoService.ActualizarAsync(id, request);
                if (empleado == null)
                    return NotFound(new { Mensaje = "El empleado no existe." });

                return Ok(empleado);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Mensaje = ex.Message });
            }
        }

        // Baja lógica: internamente solo desactiva (NEstado = 0) y bloquea el
        // UsuarioLogin asociado, no borra la fila. Mismo criterio que Roles.
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var eliminado = await _empleadoService.EliminarAsync(id);
            if (!eliminado)
                return NotFound(new { Mensaje = "El empleado no existe." });

            return NoContent();
        }
    }
}
