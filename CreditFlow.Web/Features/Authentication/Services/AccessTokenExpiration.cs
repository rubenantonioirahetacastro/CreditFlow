using System.Text;
using System.Text.Json;

namespace CreditFlow.Web.Features.Authentication.Services;

/// <summary>Lee el vencimiento («exp») del JWT que entrega la API, sin validarlo (la API lo valida en cada petición).</summary>
public static class AccessTokenExpiration
{
    public static DateTimeOffset? Read(string? token)
    {
        var partes = token?.Split('.');
        if (partes is not { Length: 3 })
            return null;

        try
        {
            var payload = partes[1].Replace('-', '+').Replace('_', '/');
            payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');

            using var json = JsonDocument.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(payload)));
            return json.RootElement.TryGetProperty("exp", out var exp) && exp.TryGetInt64(out var segundos)
                ? DateTimeOffset.FromUnixTimeSeconds(segundos)
                : null;
        }
        catch (Exception ex) when (ex is FormatException or JsonException)
        {
            return null;
        }
    }
}
