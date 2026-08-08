using CreditFlow.API.Application.Interfaces.Mantenimientos;
using CreditFlow.API.Application.Requests.Mantenimientos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CreditFlow.API.Controllers.Mantenimientos
{
    [Route("api/mantenimientos/agencias")]
    [ApiController]
    public class AgenciaController : ControllerBase
    {
        private readonly IAgenciaService _agenciaService;

        public AgenciaController(IAgenciaService agenciaService)
        {
            _agenciaService = agenciaService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Supervisor,Tecnologia")]
        public async Task<IActionResult> ObtenerTodas()
        {
            var agencias = await _agenciaService.ObtenerTodasAsync();
            return Ok(agencias);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Supervisor,Tecnologia")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var agencia = await _agenciaService.ObtenerPorIdAsync(id);
            if (agencia == null)
                return NotFound(new { Mensaje = "Agencia no encontrada." });

            return Ok(agencia);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Tecnologia")]
        public async Task<IActionResult> Crear([FromBody] CrearAgenciaRequest request)
        {
            try
            {
                var agencia = await _agenciaService.CrearAsync(request);
                return Created($"api/mantenimientos/agencias/{agencia.NCodAge}", agencia);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Mensaje = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Tecnologia")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarAgenciaRequest request)
        {
            var agencia = await _agenciaService.ActualizarAsync(id, request);
            if (agencia == null)
                return NotFound(new { Mensaje = "Agencia no encontrada." });

            return Ok(agencia);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Tecnologia")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var eliminado = await _agenciaService.EliminarAsync(id);
            if (!eliminado)
                return NotFound(new { Mensaje = "Agencia no encontrada." });

            return NoContent();
        }
    }
}
