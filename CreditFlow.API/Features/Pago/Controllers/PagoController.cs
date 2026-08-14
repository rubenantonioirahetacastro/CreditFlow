using CreditFlow.API.Features.Pago.Requests;
using CreditFlow.API.Features.Pago.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CreditFlow.API.Features.Pago.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagoController : ControllerBase
    {
        private readonly IPagoService _pagoService;

        public PagoController(IPagoService pagoService)
        {
            _pagoService = pagoService;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> RegistrarPago([FromBody] PagoRequest request)
        {
            try
            {
                var cuotasActualizadas = await _pagoService.RegistrarPagoAsync(request);
                return Ok(new
                {
                    Mensaje = "Pago registrado exitosamente.",
                    CuotasActualizadas = cuotasActualizadas
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensaje = ex.Message });
            }
        }
    }
}