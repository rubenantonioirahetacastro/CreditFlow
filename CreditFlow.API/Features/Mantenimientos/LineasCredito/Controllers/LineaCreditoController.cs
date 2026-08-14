using CreditFlow.API.Features.Mantenimientos.LineasCredito.Services;
using CreditFlow.API.Features.Mantenimientos.LineasCredito.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CreditFlow.API.Features.Mantenimientos.LineasCredito.Controllers
{
    [Route("api/mantenimientos/lineas-credito")]
    [ApiController]
    public class LineaCreditoController : ControllerBase
    {
        private readonly ILineaCreditoAdminService _lineaCreditoService;

        public LineaCreditoController(ILineaCreditoAdminService lineaCreditoService)
        {
            _lineaCreditoService = lineaCreditoService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Supervisor,Tecnologia")]
        public async Task<IActionResult> ObtenerTodas()
        {
            var lineas = await _lineaCreditoService.ObtenerTodasAsync();
            return Ok(lineas);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Supervisor,Tecnologia")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var linea = await _lineaCreditoService.ObtenerPorIdAsync(id);
            if (linea == null)
                return NotFound(new { Mensaje = "Línea de crédito no encontrada." });

            return Ok(linea);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Tecnologia")]
        public async Task<IActionResult> Crear([FromBody] CrearLineaCreditoRequest request)
        {
            try
            {
                var linea = await _lineaCreditoService.CrearAsync(request, ObtenerUsuarioActual());
                return Created($"api/mantenimientos/lineas-credito/{linea.NCodLinea}", linea);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Mensaje = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Tecnologia")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarLineaCreditoRequest request)
        {
            try
            {
                var linea = await _lineaCreditoService.ActualizarAsync(id, request, ObtenerUsuarioActual());
                if (linea == null)
                    return NotFound(new { Mensaje = "Línea de crédito no encontrada." });

                return Ok(linea);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Mensaje = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Tecnologia")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var eliminado = await _lineaCreditoService.EliminarAsync(id);
                if (!eliminado)
                    return NotFound(new { Mensaje = "Línea de crédito no encontrada." });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Mensaje = ex.Message });
            }
        }

        private string? ObtenerUsuarioActual() =>
            User.Identity?.Name ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    }
}
