using System.Security.Claims;
using CreditFlow.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace CreditFlow.Web.Endpoints;

// Endpoints no interactivos (fuera del circuito de Blazor) para poder llamar
// HttpContext.SignInAsync/SignOutAsync, que requieren escribir la respuesta
// HTTP inicial y fallan si se invocan desde un componente ya interactivo.
public static class AuthEndpoints
{
    // PENDIENTE CONOCIDO: no hay rate limiting por IP aquí ni en CreditFlow.API.
    // El bloqueo por 3 intentos fallidos (UsuarioLogin.IntentosFallidos/Bloqueado)
    // ya vive del lado de CreditFlow.API (AuthController.Login_web) y es por
    // cuenta, no por origen — no protege contra fuerza bruta distribuida entre
    // muchas cuentas ni contra llamadas directas a la API. Si se agrega, el
    // lugar natural es CreditFlow.API vía Microsoft.AspNetCore.RateLimiting.
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/login", async (
            HttpContext context,
            [FromForm] string documento,
            [FromForm] string password,
            IAuthService authService) =>
        {
            var response = await authService.LoginAsync(documento, password);

            if (!response.Exito)
            {
                var mensaje = Uri.EscapeDataString(response.Mensaje ?? "No se pudo iniciar sesión.");
                return Results.Redirect($"/login?error={mensaje}");
            }

            var roles = await authService.ObtenerRolesAsync();
            var nombreRol = roles.FirstOrDefault(r => r.IdRol == response.IdRol)?.Nombre ?? response.IdRol.ToString();

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, documento),
                new(ClaimTypes.NameIdentifier, documento),
                new("IdPersona", response.IdPersona.ToString()),
                new(ClaimTypes.Role, nombreRol),
                new("AccessToken", response.Token ?? string.Empty)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Results.Redirect(response.BTemporal ? "/password-temporal" : "/");
        });

        app.MapPost("/logout", async (HttpContext context) =>
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.Redirect("/login");
        });
    }
}
