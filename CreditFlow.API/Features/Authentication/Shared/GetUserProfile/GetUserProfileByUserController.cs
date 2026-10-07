using System.Security.Claims;
using CreditFlow.API.Core.Errors;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Authentication.Shared.GetUserProfile;

[Route("api/Auth")]
[ApiController]
public sealed class GetUserProfileByUserController(DbNegocioContext context) : ControllerBase
{
    private static readonly ErrorDefinition ProfileNotFound = new(
        "auth_profile_not_found",
        "No se encontró el perfil del usuario.");

    /// <summary>
    /// Perfil de un usuario (cliente o empleado) a partir de su IdUsuario. Solo puede consultarse
    /// el propio: el id de la ruta debe coincidir con el usuario del token.
    /// </summary>
    [HttpGet("perfil/{idUsuario:int}")]
    [Authorize]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserProfileResponse>> Get(int idUsuario, CancellationToken cancellationToken)
    {
        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var tokenUserId)
            || tokenUserId != idUsuario)
            throw new ResourceNotFoundException(ProfileNotFound);

        var person = await context.Personas.AsNoTracking()
            .Where(item => item.IdUsuario == idUsuario)
            .Select(item => new UserProfileResponse(
                idUsuario,
                "cliente",
                item.IdPersona,
                null,
                item.CNombres,
                item.CDocumento,
                item.CCorreo,
                item.CTelefono,
                item.CCelular,
                context.UsuarioLogins.Where(login => login.IdUsuario == idUsuario).Select(login => login.VFoto).FirstOrDefault() ?? item.VFotoPerfil))
            .FirstOrDefaultAsync(cancellationToken);

        if (person is not null)
            return Ok(person);

        var employee = await context.Empleados.AsNoTracking()
            .Where(item => item.IdUsuario == idUsuario)
            .Select(item => new UserProfileResponse(
                idUsuario,
                "empleado",
                null,
                item.IdEmpleado,
                item.CNombres,
                item.CDocumento,
                item.CCorreo,
                item.CTelefono,
                null,
                context.UsuarioLogins.Where(login => login.IdUsuario == idUsuario).Select(login => login.VFoto).FirstOrDefault()))
            .FirstOrDefaultAsync(cancellationToken);

        return employee is null
            ? throw new ResourceNotFoundException(ProfileNotFound)
            : Ok(employee);
    }
}
