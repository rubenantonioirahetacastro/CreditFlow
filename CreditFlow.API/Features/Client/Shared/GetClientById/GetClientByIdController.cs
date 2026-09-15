using CreditFlow.API.Core.Diagnostics;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Client.Shared.GetClientById;

[Route("api/BuscarCliente")]
[ApiController]
public class GetClientByIdController(
    DbNegocioContext context,
    IErrorLogger errorLogger) : ControllerBase
{
    [HttpGet("GetClientePorId/{idPersona}")]
    public async Task<IActionResult> GetClientById(int idPersona)
    {
        try
        {
            if (idPersona <= 0)
                return BadRequest("IdPersona inválido.");

            var person = await context.Personas
                .AsNoTracking()
                .Where(item => item.IdPersona == idPersona)
                .Select(item => new
                {
                    item.IdPersona,
                    item.NTipoDocumento,
                    item.CDocumento,
                    item.DFechaExpedicion,
                    item.DFechaVencimiento,
                    item.NDepartamentoDoc,
                    item.NMunicipioDoc,
                    item.CNombres,
                    item.CPrimerApellido,
                    item.CSegundoApellido,
                    item.NSexo,
                    item.NNacionalidad,
                    item.DFechaNacimiento,
                    item.NDepartamentoNacimiento,
                    item.NMunicipioNacimiento,
                    item.NEstadoCivil,
                    item.NProfesion,
                    item.NEscolaridad,
                    item.CCorreo,
                    item.CTelefono,
                    item.CCelular
                })
                .FirstOrDefaultAsync();

            if (person == null)
                return NotFound($"Persona {idPersona} no existe en la base de datos");

            var photos = await context.FotoIds
                .AsNoTracking()
                .Where(item => item.IdPersona == idPersona)
                .Select(item => new { item.IdFoto, item.VFoto, item.NTipoFoto })
                .ToListAsync();

            return Ok(new { persona = person, fotos = photos });
        }
        catch (Exception exception)
        {
            await errorLogger.LogAsync(exception);
            return StatusCode(500, "Ocurrió un error al procesar la solicitud.");
        }
    }
}
