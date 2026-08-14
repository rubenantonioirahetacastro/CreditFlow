using CreditFlow.API.Infrastructure.Data;
using CreditFlow.API.Features.EvaluacionCredito.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.EvaluacionCredito.Controllers
{
    [Route("api/Credito")]
    [ApiController]
    public class EvaluacionCreditoController : ControllerBase
    {
        private readonly DbNegocioContext _context;

        public EvaluacionCreditoController(DbNegocioContext context)
        {
            _context = context;
        }

        [HttpPut("actualizar-evaluacion")]
        [Authorize]
        public async Task<IActionResult> ActualizarEvaluacion([FromBody] ActualizarEvaluacionRequest request)
        {
            try
            {
                var credito = await _context.Creditos
                    .FirstOrDefaultAsync(c => c.NCodAge == request.NCodAge && c.NCodCred == request.NCodCred);

                if (credito == null)
                    return NotFound(new { Mensaje = "No se encontró el crédito indicado." });

                credito.NEstado = request.NEstado;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensaje = $"Error interno del servidor: {ex.Message}" });
            }
        }
    }
}
