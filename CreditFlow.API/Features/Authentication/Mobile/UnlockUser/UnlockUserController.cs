using System.Security.Cryptography;
using CreditFlow.API.Core.Email;
using CreditFlow.API.Core.Security;
using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Features.Authentication.Shared.Session;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Authentication.Mobile.UnlockUser;

[Route("api/Auth")]
[ApiController]
public class UnlockUserController : ControllerBase
{
    private static readonly TimeSpan TokenExpiration = TimeSpan.FromMinutes(15);

    private readonly DbNegocioContext _context;
    private readonly IEmailService _emailService;
    private readonly IUserSessionService _userSessionService;

    public UnlockUserController(
        DbNegocioContext context,
        IEmailService emailService,
        IUserSessionService userSessionService)
    {
        _context = context;
        _emailService = emailService;
        _userSessionService = userSessionService;
    }

    [HttpPost("request-unlock-app")]
    [AllowAnonymous]
    public async Task<IActionResult> RequestUnlock([FromBody] UnlockRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Usuario))
            return BadRequest(new { Exito = false, Mensaje = "Usuario es requerido" });

        var user = await _context.UsuarioLogins.FirstOrDefaultAsync(u => u.CDocumento == request.Usuario);
        if (user == null)
            return NotFound(new { Exito = false, Mensaje = "Usuario no existe" });

        var session = await _userSessionService.GetAsync(user.IdUsuario, user.CDocumento);

        if (!session.RoleIds.Contains(RoleIds.Client))
            return Unauthorized(new { Exito = false, Mensaje = "Solo los usuarios con rol Usuario pueden solicitar el desbloqueo desde la app." });

        if (user.Bloqueado != 1)
            return BadRequest(new { Exito = false, Mensaje = "El usuario no está bloqueado." });

        if (string.IsNullOrWhiteSpace(user.CCorreo))
            return BadRequest(new { Exito = false, Mensaje = "El usuario no tiene correo registrado para recibir el token." });

        var token = GenerateToken();

        user.Token = int.Parse(token);
        user.TokenTime = DateTime.UtcNow;
        user.TokenCheck = false;

        _context.UsuarioLogins.Update(user);

        await _context.PasswordChangeAudits.AddAsync(new PasswordChangeAudit
        {
            IdUsuario = user.IdUsuario,
            IdPersona = session.PersonId,
            Usuario = user.CDocumento,
            Exito = true,
            FechaAttempt = DateTime.UtcNow,
            Ip = HttpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = Request.Headers["User-Agent"].ToString(),
            IntentosFallidos = user.IntentosFallidos,
            Bloqueado = true,
            Observacion = "Se generó token de desbloqueo desde la app"
        });

        await _context.SaveChangesAsync();

        var subject = "Token de desbloqueo de cuenta";
        var body = $"Estimado(a) usuario:\n\n" +
                   $"Hemos generado un token para desbloquear su cuenta: {token}.\n" +
                   $"Este código vence en {TokenExpiration.TotalMinutes:0} minutos.\n\n" +
                   "Por favor, ingréselo en la aplicación para completar el proceso.";

        await _emailService.SendAsync(user.CCorreo, subject, body);

        return Ok(new { Exito = true, Mensaje = "Se envió un token de desbloqueo al correo registrado." });
    }

    [HttpPost("confirm-unlock-app")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmUnlock([FromBody] UnlockConfirmRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Usuario) || string.IsNullOrWhiteSpace(request.Token))
            return BadRequest(new { Exito = false, Mensaje = "Usuario y Token son obligatorios" });

        var user = await _context.UsuarioLogins.FirstOrDefaultAsync(u => u.CDocumento == request.Usuario);
        if (user == null)
            return NotFound(new { Exito = false, Mensaje = "Usuario no existe" });

        var session = await _userSessionService.GetAsync(user.IdUsuario, user.CDocumento);

        if (!session.RoleIds.Contains(RoleIds.Client))
            return Unauthorized(new { Exito = false, Mensaje = "Solo los usuarios con rol Usuario pueden usar este flujo." });

        if (user.Bloqueado != 1)
            return BadRequest(new { Exito = false, Mensaje = "El usuario no está bloqueado." });

        if (user.Token == null || user.TokenTime == null)
            return BadRequest(new { Exito = false, Mensaje = "No existe un token de desbloqueo vigente." });

        if (user.TokenCheck == true)
            return BadRequest(new { Exito = false, Mensaje = "El token ya fue utilizado." });

        if (DateTime.UtcNow - user.TokenTime.Value > TokenExpiration)
        {
            user.Token = null;
            user.TokenTime = null;
            user.TokenCheck = null;
            _context.UsuarioLogins.Update(user);
            await _context.SaveChangesAsync();
            return BadRequest(new { Exito = false, Mensaje = "El token de desbloqueo expiró." });
        }

        if (!int.TryParse(request.Token.Trim(), out var enteredToken) || enteredToken != user.Token.Value)
            return BadRequest(new { Exito = false, Mensaje = "Token inválido" });

        user.Bloqueado = 0;
        user.IntentosFallidos = 0;
        user.FechaBloqueo = null;
        user.Token = null;
        user.TokenTime = null;
        user.TokenCheck = true;

        _context.UsuarioLogins.Update(user);

        await _context.PasswordChangeAudits.AddAsync(new PasswordChangeAudit
        {
            IdUsuario = user.IdUsuario,
            IdPersona = session.PersonId,
            Usuario = user.CDocumento,
            Exito = true,
            FechaAttempt = DateTime.UtcNow,
            Ip = HttpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = Request.Headers["User-Agent"].ToString(),
            IntentosFallidos = user.IntentosFallidos,
            Bloqueado = false,
            Observacion = "Usuario desbloqueado desde la app con token válido"
        });

        await _context.SaveChangesAsync();

        return Ok(new { Exito = true, Mensaje = "Usuario desbloqueado correctamente" });
    }

    private static string GenerateToken() =>
        RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");

}
