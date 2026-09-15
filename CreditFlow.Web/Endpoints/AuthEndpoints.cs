using System.Security.Claims;
using CreditFlow.Web.Core.Security;
using CreditFlow.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace CreditFlow.Web.Endpoints;

public static class AuthEndpoints
{
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

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, documento),
                new(ClaimTypes.NameIdentifier, documento),
                new(CustomClaimTypes.AccessToken, response.Token ?? string.Empty)
            };

            var roleIds = response.IdRoles.Count > 0
                ? response.IdRoles
                : [response.IdRol];
            foreach (var roleId in roleIds.Where(roleId => roleId > 0).Distinct())
                claims.Add(new Claim(CustomClaimTypes.RoleId, roleId.ToString()));

            if (response.IdPersona.HasValue)
                claims.Add(new Claim(CustomClaimTypes.PersonId, response.IdPersona.Value.ToString()));

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
