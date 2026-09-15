using CreditFlow.API.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace CreditFlow.API.Features.Client.Shared.UpdateContact;

[Route("api/Persona")]
[ApiController]
public class UpdateContactController(DbNegocioContext context) : ControllerBase
{
    [HttpPut("{id}/contacto")]
    public async Task<IActionResult> UpdateContact(int id, [FromBody] UpdateContactRequest request)
    {
        if (request == null)
        {
            return BadRequest(
                "No se pudo completar la actualización porque el cuerpo de la solicitud es nulo");
        }

        var existing = await context.Personas.FindAsync(id);
        if (existing == null)
            return NotFound($"Persona {id} no existe en la base de datos");

        existing.CCorreo = request.CCorreo;
        existing.CTelefono = request.CTelefono;
        existing.CCelular = request.CCelular;

        context.Personas.Update(existing);
        await context.SaveChangesAsync();

        return Ok("Datos de contacto actualizados correctamente");
    }
}
