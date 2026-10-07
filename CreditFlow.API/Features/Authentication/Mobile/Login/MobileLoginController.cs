using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Core.Errors;
using CreditFlow.API.Core.Security;
using CreditFlow.API.Features.Authentication.Shared.Login;
using CreditFlow.API.Features.Authentication.Shared.Errors;
using CreditFlow.API.Features.Authentication.Shared.Session;
using CreditFlow.API.Features.Authentication.Shared.Token;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Authentication.Mobile.Login;

[Route("api/Auth")]
[ApiController]
public class MobileLoginController : ControllerBase
{
    private readonly DbNegocioContext _context;
    private readonly IUserSessionService _userSessionService;
    private readonly IJwtTokenService _jwtTokenService;

    public MobileLoginController(
        DbNegocioContext context,
        IUserSessionService userSessionService,
        IJwtTokenService jwtTokenService)
    {
        _context = context;
        _userSessionService = userSessionService;
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost("login-cliente")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var rawDocument = request.Documento.Trim();

        var user = await _context.UsuarioLogins.FirstOrDefaultAsync(u => u.CDocumento == rawDocument);
        if (user == null)
        {
            var normalizedDocument = rawDocument.Replace("-", string.Empty).Replace(" ", string.Empty).ToLower();
            user = await _context.UsuarioLogins.FirstOrDefaultAsync(u =>
                (u.CDocumento ?? string.Empty).Replace("-", string.Empty).Replace(" ", string.Empty).ToLower() == normalizedDocument);
        }

        if (user == null)
            return Unauthorized(CreateError(AuthenticationErrors.InvalidCredentials));

        if (user.Bloqueado == 1)
            return BadRequest(CreateError(AuthenticationErrors.UserBlocked));

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        {
            user.IntentosFallidos = (user.IntentosFallidos ?? 0) + 1;

            if (user.IntentosFallidos >= 3)
            {
                user.Bloqueado = 1;
                user.FechaBloqueo = int.Parse(DateTime.UtcNow.ToString("yyyyMMdd"));

                await _context.PasswordChangeAudits.AddAsync(new PasswordChangeAudit
                {
                    IdUsuario = user.IdUsuario,
                    IdPersona = null,
                    Usuario = user.CDocumento,
                    Exito = false,
                    FechaAttempt = DateTime.UtcNow,
                    Ip = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = Request.Headers["User-Agent"].ToString(),
                    IntentosFallidos = user.IntentosFallidos,
                    Bloqueado = true,
                    FechaBloqueo = DateTime.UtcNow,
                    MotivoBloqueo = "Tres intentos fallidos",
                    Observacion = "Cuenta bloqueada por intentos fallidos en login"
                });
            }

            _context.UsuarioLogins.Update(user);
            await _context.SaveChangesAsync();

            return Unauthorized(CreateError(AuthenticationErrors.InvalidCredentials));
        }

        user.IntentosFallidos = 0;
        user.UltimoLogin = DateTime.UtcNow;
        _context.UsuarioLogins.Update(user);

        var session = await _userSessionService.GetAsync(user.IdUsuario, user.CDocumento);

        if (!session.RoleIds.Any(RoleIds.MobileAccess.Contains))
        {
            await _context.PasswordChangeAudits.AddAsync(new PasswordChangeAudit
            {
                IdUsuario = user.IdUsuario,
                IdPersona = session.PersonId,
                Usuario = user.CDocumento,
                Exito = false,
                FechaAttempt = DateTime.UtcNow,
                Ip = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = Request.Headers["User-Agent"].ToString(),
                IntentosFallidos = user.IntentosFallidos,
                Bloqueado = false,
                Observacion = "Intento de login rechazado: rol no permitido"
            });
            await _context.SaveChangesAsync();

            return Unauthorized(CreateError(AuthenticationErrors.RoleNotAllowed));
        }

        var tokenString = _jwtTokenService.Create(session);

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
            Observacion = "Login exitoso"
        });

        await _context.SaveChangesAsync();

        return Ok(new
        {
            Exito = true,
            Mensaje = "Autenticación exitosa",
            Token = tokenString,
            IdPersona = session.PersonId,
            bTemporal = user.BContrasenaTemporal == true,
            IdRol = session.RoleId,
            IdRoles = session.RoleIds
        });
    }

    private static object CreateError(ErrorDefinition error) => new
    {
        Exito = false,
        Codigo = error.Code,
        Mensaje = error.Message
    };
}
