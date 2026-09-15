using Microsoft.AspNetCore.Mvc;

namespace CreditFlow.API.Features.Credit.Shared.Calendar;

[Route("api/Calendario")]
[ApiController]
public sealed class GenerateCalendarController(
    ICalendarioService calendarioService) : ControllerBase
{
    [HttpGet("generar/{nCodAge}/{nCodCred}")]
    public async Task<IActionResult> Generate(int nCodAge, int nCodCred)
    {
        var calendario = await calendarioService.GenerarCalendarioAsync(nCodAge, nCodCred);

        if (calendario == null || calendario.Count == 0)
            return NotFound(new { Mensaje = "No se pudo generar el calendario o no se encontraron cuotas." });

        return Ok(calendario);
    }
}
