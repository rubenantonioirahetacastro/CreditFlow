namespace CreditFlow.Web.Features.Authentication.Services;

/// <summary>
/// «Recordarme» del login: guarda solo el documento del usuario en una cookie HttpOnly para prellenarlo.
/// La contraseña nunca se guarda; eso queda a cargo del administrador de contraseñas del navegador.
/// </summary>
public static class RememberedUser
{
    private const string CookieName = "CreditFlow.RememberedUser";
    private static readonly TimeSpan Duration = TimeSpan.FromDays(30);

    public static string? Read(HttpContext context) =>
        context.Request.Cookies.TryGetValue(CookieName, out var documento) && !string.IsNullOrWhiteSpace(documento)
            ? documento
            : null;

    public static void Save(HttpContext context, string documento) =>
        context.Response.Cookies.Append(CookieName, documento.Trim(), new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            IsEssential = true,
            Expires = DateTimeOffset.UtcNow.Add(Duration)
        });

    public static void Clear(HttpContext context) =>
        context.Response.Cookies.Delete(CookieName, new CookieOptions { Secure = true, SameSite = SameSiteMode.Lax });
}
