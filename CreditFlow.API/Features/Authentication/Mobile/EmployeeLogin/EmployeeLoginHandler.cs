using CreditFlow.API.Core.Errors;
using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Features.Authentication.Shared.Errors;
using CreditFlow.API.Features.Authentication.Shared.Session;
using CreditFlow.API.Features.Authentication.Shared.Token;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Authentication.Mobile.EmployeeLogin;

public sealed class EmployeeLoginHandler(
    DbNegocioContext context,
    IUserSessionService userSessionService,
    IJwtTokenService jwtTokenService,
    IHttpContextAccessor httpContextAccessor)
{
    private const int MaxFailedAttempts = 3;

    public async Task<EmployeeLoginResponse> ExecuteAsync(
        EmployeeLoginRequest request,
        CancellationToken cancellationToken)
    {
        var userCode = request.CCodUsu.Trim();

        var user = await context.UsuarioLogins
            .FirstOrDefaultAsync(item => item.CCodUsu == userCode, cancellationToken);

        var employee = user is null
            ? null
            : await context.Empleados
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.IdUsuario == user.IdUsuario, cancellationToken);

        if (user is null || employee is null)
            throw new UnauthorizedAppException(AuthenticationErrors.InvalidCredentials);

        if (user.Bloqueado == 1)
            throw new BusinessRuleException(AuthenticationErrors.UserBlocked);

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        {
            await RegisterFailedAttemptAsync(user, employee, cancellationToken);
            throw new UnauthorizedAppException(AuthenticationErrors.InvalidCredentials);
        }

        var session = await userSessionService.GetAsync(user.IdUsuario, user.CDocumento);
        if (session.RoleIds.Count == 0)
        {
            await AddAuditAsync(user, employee, false, "Intento de login de empleado rechazado: sin rol activo", cancellationToken);
            throw new UnauthorizedAppException(AuthenticationErrors.ActiveRoleRequired);
        }

        user.IntentosFallidos = 0;
        user.UltimoLogin = DateTime.UtcNow;

        var token = jwtTokenService.Create(session);

        await AddAuditAsync(user, employee, true, "Login de empleado exitoso", cancellationToken);

        return new EmployeeLoginResponse(
            true,
            "Autenticación exitosa",
            token,
            session.PersonId,
            user.BContrasenaTemporal == true,
            session.RoleId,
            session.RoleIds);
    }

    private async Task RegisterFailedAttemptAsync(
        UsuarioLogin user,
        Empleado employee,
        CancellationToken cancellationToken)
    {
        user.IntentosFallidos = (user.IntentosFallidos ?? 0) + 1;

        var blocked = user.IntentosFallidos >= MaxFailedAttempts;
        if (blocked)
        {
            user.Bloqueado = 1;
            user.FechaBloqueo = int.Parse(DateTime.UtcNow.ToString("yyyyMMdd"));
        }

        await AddAuditAsync(
            user,
            employee,
            false,
            blocked ? "Cuenta bloqueada por intentos fallidos en login" : "Contraseña incorrecta en login de empleado",
            cancellationToken,
            blocked);
    }

    private async Task AddAuditAsync(
        UsuarioLogin user,
        Empleado employee,
        bool success,
        string note,
        CancellationToken cancellationToken,
        bool blocked = false)
    {
        var http = httpContextAccessor.HttpContext;

        context.PasswordChangeAudits.Add(new PasswordChangeAudit
        {
            IdUsuario = user.IdUsuario,
            IdPersona = null,
            Usuario = user.CCodUsu,
            Exito = success,
            FechaAttempt = DateTime.UtcNow,
            Ip = http?.Connection.RemoteIpAddress?.ToString(),
            UserAgent = http?.Request.Headers["User-Agent"].ToString(),
            IntentosFallidos = user.IntentosFallidos,
            Bloqueado = blocked,
            FechaBloqueo = blocked ? DateTime.UtcNow : null,
            MotivoBloqueo = blocked ? "Tres intentos fallidos" : null,
            Observacion = note
        });

        await context.SaveChangesAsync(cancellationToken);
    }
}
