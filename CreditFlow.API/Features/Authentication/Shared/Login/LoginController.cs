using CreditFlow.API.Features.Authentication.Shared.Session;
using CreditFlow.API.Features.Authentication.Shared.Token;
using CreditFlow.API.Core.Errors;
using CreditFlow.API.Features.Authentication.Shared.Errors;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Authentication.Shared.Login;

[ApiController]
[Route("api/Token")]
public sealed class LoginController(
    DbNegocioContext context,
    IUserSessionService userSessionService,
    IJwtTokenService jwtTokenService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var rawDocument = request.Documento.Trim();
        var password = request.Password;

        var user = await context.UsuarioLogins
            .FirstOrDefaultAsync(item => item.CDocumento == rawDocument);

        if (user == null)
        {
            var normalizedDocument = rawDocument
                .Replace("-", string.Empty)
                .Replace(" ", string.Empty)
                .ToLower();
            user = await context.UsuarioLogins.FirstOrDefaultAsync(item =>
                (item.CDocumento ?? string.Empty)
                    .Replace("-", string.Empty)
                    .Replace(" ", string.Empty)
                    .ToLower() == normalizedDocument);
        }

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            return Unauthorized(new ApiErrorResponse(
                AuthenticationErrors.InvalidCredentials.Code,
                AuthenticationErrors.InvalidCredentials.Message));

        var session = await userSessionService.GetAsync(user.IdUsuario, user.CDocumento);
        if (session.RoleIds.Count == 0)
            return Unauthorized(new ApiErrorResponse(
                AuthenticationErrors.ActiveRoleRequired.Code,
                AuthenticationErrors.ActiveRoleRequired.Message));

        return Ok(new
        {
            token = jwtTokenService.Create(session)
        });
    }
}
