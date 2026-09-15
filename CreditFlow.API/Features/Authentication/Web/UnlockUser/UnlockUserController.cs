using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Authentication.Web.UnlockUser;

[Route("api/Auth")]
[ApiController]
public class UnlockUserController(DbNegocioContext context) : ControllerBase
{
    [HttpPost("unlock")]
    public async Task<IActionResult> UnlockUser([FromBody] UnlockUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Usuario))
            return BadRequest(new { Exito = false, Mensaje = "Usuario es requerido" });

        var user = await context.UsuarioLogins.FirstOrDefaultAsync(u => u.CDocumento == request.Usuario);
        if (user == null)
            return NotFound(new { Exito = false, Mensaje = "Usuario no existe" });

        user.Bloqueado = 0;
        user.IntentosFallidos = 0;
        user.FechaBloqueo = null;

        context.UsuarioLogins.Update(user);

        await context.PasswordChangeAudits.AddAsync(new PasswordChangeAudit
        {
            IdUsuario = user.IdUsuario,
            IdPersona = user.IdUsuario,
            Usuario = user.CDocumento,
            Exito = true,
            FechaAttempt = DateTime.UtcNow,
            Ip = HttpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = Request.Headers["User-Agent"].ToString(),
            IntentosFallidos = user.IntentosFallidos,
            Bloqueado = false,
            Observacion = string.IsNullOrWhiteSpace(request.Observacion)
                ? "Usuario desbloqueado por administrador"
                : request.Observacion
        });

        await context.SaveChangesAsync();

        return Ok(new { Exito = true, Mensaje = "Usuario desbloqueado correctamente" });
    }
}
