using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Authentication.Mobile.ChangeTempPassword;

[Route("api/Auth")]
[ApiController]
public sealed class ChangeTempPasswordController(
    DbNegocioContext context) : ControllerBase
{
    [HttpPost("change-temp-password")]
    public async Task<IActionResult> Change(
        [FromBody] ChangeTempPasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Usuario) ||
            string.IsNullOrWhiteSpace(request.ContrasenaNueva))
        {
            return BadRequest(new
            {
                Exito = false,
                Mensaje = "Usuario y ContraseñaNueva son obligatorios."
            });
        }

        var user = await context.UsuarioLogins
            .FirstOrDefaultAsync(item => item.CDocumento == request.Usuario);

        if (user == null)
            return NotFound(new { Exito = false, Mensaje = "Usuario no existe" });

        if (user.Bloqueado == 1)
            return BadRequest(new { Exito = false, Mensaje = "Usuario bloqueado" });

        if (user.BContrasenaTemporal != true)
        {
            return BadRequest(new
            {
                Exito = false,
                Mensaje = "El usuario no tiene contraseña temporal"
            });
        }

        user.IntentosFallidos = 0;
        user.Password = BCrypt.Net.BCrypt.HashPassword(request.ContrasenaNueva);
        user.UltimoLogin = DateTime.UtcNow;
        user.BContrasenaTemporal = false;
        user.DFechaContrasenaTemporal = null;

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
            Observacion = "Cambio de contraseña temporal exitoso"
        });

        await context.SaveChangesAsync();

        return Ok(new
        {
            Exito = true,
            Mensaje = "Contraseña actualizada correctamente",
            Cambiada = true
        });
    }
}
