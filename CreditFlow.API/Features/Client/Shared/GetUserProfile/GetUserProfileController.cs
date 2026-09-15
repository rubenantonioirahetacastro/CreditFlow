using CreditFlow.API.Core.Security;
using CreditFlow.API.Core.Diagnostics;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Client.Shared.GetUserProfile;

[Route("api/BuscarCliente")]
[ApiController]
public class GetUserProfileController(
    DbNegocioContext context,
    IErrorLogger errorLogger) : ControllerBase
{
    [HttpGet("GetUsuarioPorId/{idPersona}")]
    public async Task<IActionResult> GetUserProfile(int idPersona)
    {
        try
        {
            if (idPersona <= 0)
                return BadRequest("IdPersona inválido.");

            var user = await (
                from person in context.Personas.AsNoTracking()
                join login in context.UsuarioLogins.AsNoTracking()
                    on person.IdUsuario equals login.IdUsuario
                let hasClientRole = context.UsuarioRoles.AsNoTracking()
                    .Any(item =>
                        item.IdUsuario == login.IdUsuario &&
                        item.IdRol == RoleIds.Client &&
                        item.IdRolNavigation.Activo)
                where person.IdPersona == idPersona &&
                      hasClientRole
                select new
                {
                    person.IdPersona,
                    person.CNombres,
                    person.CDocumento,
                    person.CCorreo,
                    person.CTelefono,
                    person.CCelular
                }
            ).FirstOrDefaultAsync();

            if (user == null)
                return NotFound($"Persona {idPersona} no encontrada o rol no permitido.");

            return Ok(user);
        }
        catch (Exception exception)
        {
            await errorLogger.LogAsync(exception);
            return StatusCode(500, "Ocurrió un error al procesar la solicitud.");
        }
    }
}
