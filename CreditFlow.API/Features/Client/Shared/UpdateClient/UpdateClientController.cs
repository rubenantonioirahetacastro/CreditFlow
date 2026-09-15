using CreditFlow.API.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace CreditFlow.API.Features.Client.Shared.UpdateClient;

[Route("api/Persona")]
[ApiController]
public class UpdateClientController(DbNegocioContext context) : ControllerBase
{
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateClient(int id, [FromBody] UpdateClientRequest request)
    {
        if (request == null)
            return BadRequest("Persona inválida");

        if (request.IdPersona != 0 && request.IdPersona != id)
            return BadRequest("El IdPersona del cuerpo no coincide con el id de la ruta");

        var existing = await context.Personas.FindAsync(id);
        if (existing == null)
            return NotFound($"Persona {request.CNombres} no existe en la base de datos ");

        existing.NTipoDocumento = request.NTipoDocumento;
        existing.CDocumento = request.CDocumento;
        existing.DFechaExpedicion = request.DFechaExpedicion;
        existing.DFechaVencimiento = request.DFechaVencimiento;
        existing.NDepartamentoDoc = request.NDepartamentoDoc;
        existing.NMunicipioDoc = request.NMunicipioDoc;
        existing.CNombres = request.CNombres;
        existing.CPrimerApellido = request.CPrimerApellido;
        existing.CSegundoApellido = request.CSegundoApellido;
        existing.NSexo = request.NSexo;
        existing.NNacionalidad = request.NNacionalidad;
        existing.DFechaNacimiento = request.DFechaNacimiento;
        existing.NDepartamentoNacimiento = request.NDepartamentoNacimiento;
        existing.NMunicipioNacimiento = request.NMunicipioNacimiento;
        existing.NEstadoCivil = request.NEstadoCivil;
        existing.NProfesion = request.NProfesion;
        existing.NEscolaridad = request.NEscolaridad;
        existing.CCorreo = request.CCorreo;
        existing.CTelefono = request.CTelefono;
        existing.CCelular = request.CCelular;

        context.Personas.Update(existing);
        await context.SaveChangesAsync();

        return Ok("Datos actualizados correctamente");
    }
}
