using CreditFlow.API.Core.Email;
using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Authentication.Mobile.ForgotPassword;

[Route("api/Auth")]
[ApiController]
public sealed class ForgotPasswordController(
    DbNegocioContext context,
    IEmailService emailService) : ControllerBase
{
    [HttpPost("reveal-password")]
    public async Task<IActionResult> RequestTemporaryPassword(
        [FromBody] ForgotPasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Usuario))
            return BadRequest(new { Exito = false, Mensaje = "Usuario es requerido" });

        var user = await context.UsuarioLogins
            .FirstOrDefaultAsync(item => item.CDocumento == request.Usuario);

        if (user == null)
            return NotFound(new { Exito = false, Mensaje = "Usuario no existe" });

        if (user.Bloqueado == 1)
            return BadRequest(new { Exito = false, Mensaje = "Usuario bloqueado" });

        var roleNames = await context.UsuarioRoles
            .Where(item => item.IdUsuario == user.IdUsuario)
            .Include(item => item.IdRolNavigation)
            .Select(item => item.IdRolNavigation.Nombre)
            .ToListAsync();

        const string characters = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var random = new Random();
        var temporaryPassword = new string(Enumerable.Repeat(characters, 8)
            .Select(value => value[random.Next(value.Length)])
            .ToArray());

        user.Password = BCrypt.Net.BCrypt.HashPassword(temporaryPassword);
        user.IntentosFallidos = 0;
        user.Bloqueado = 0;
        user.FechaBloqueo = null;
        user.BContrasenaTemporal = true;
        user.DFechaContrasenaTemporal = DateTime.UtcNow;

        context.UsuarioLogins.Update(user);

        var joinedRoles = roleNames.Count > 0
            ? string.Join(", ", roleNames)
            : "(sin roles)";

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
            Observacion = $"Se generó contraseña temporal por administrador. Roles asignados: {joinedRoles}"
        });

        await context.SaveChangesAsync();

        if (request.EnviarCorreo)
        {
            var subject = "Contraseña temporal generada";
            var body = $"Su contraseña temporal es: {temporaryPassword}. Por favor cámbiela en su primer ingreso." +
                       $"\nRoles asignados: {joinedRoles}";
            await emailService.SendAsync(user.CCorreo, subject, body);
        }

        return Ok(new
        {
            Exito = true,
            Mensaje = request.EnviarCorreo
                ? "Contraseña temporal enviada por correo"
                : "Contraseña temporal generada (no enviada)",
            Roles = roleNames
        });
    }
}
