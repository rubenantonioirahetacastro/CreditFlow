using CreditFlow.API.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Client.Shared.CheckClientExists;

[Route("api/BuscarCliente")]
[ApiController]
public class CheckClientExistsController(DbNegocioContext context) : ControllerBase
{
    [HttpGet("buscarcliente")]
    public async Task<IActionResult> CheckClientExists([FromQuery] string cDocumento)
    {
        if (string.IsNullOrWhiteSpace(cDocumento))
            return BadRequest("Documento es obligatorio.");

        var exists = await (
            from person in context.Personas.AsNoTracking()
            join credit in context.Creditos.AsNoTracking()
                on person.IdPersona equals credit.IdPersona
            where person.CDocumento == cDocumento
            select 1
        ).AnyAsync();

        return Ok(exists ? 1 : 0);
    }
}
