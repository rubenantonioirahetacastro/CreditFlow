using CreditFlow.API.Features.Credit.Shared.Calendar;
using Microsoft.AspNetCore.Mvc;

namespace CreditFlow.API.Features.Simulator.Mobile.CalculateInstallment;

[Route("api/Calendario")]
[ApiController]
public sealed class CalculateInstallmentController(
    ICalendarioService calendarioService) : ControllerBase
{
    [HttpGet("calcular-cuota")]
    public async Task<IActionResult> Calculate(
        [FromQuery] decimal nCapital,
        [FromQuery] int nPlazo,
        [FromQuery] int nSubProd,
        [FromQuery] int nCodAge)
    {
        if (nCapital <= 0 || nPlazo <= 0)
            return BadRequest(new { Mensaje = "El capital y el plazo deben ser mayores a 0." });

        if (nCodAge <= 0)
            return BadRequest(new { Mensaje = "El código de agencia (nCodAge) es obligatorio." });

        var calendario = await calendarioService.ProyectarCalendarioAsync(
            nCapital,
            nPlazo,
            nSubProd,
            nCodAge);
        var cuotaEstimada = calendario.FirstOrDefault()?.NTotalCuota ?? 0;

        return Ok(new
        {
            CuotaEstimada = cuotaEstimada,
            Calendario = calendario
        });
    }
}
