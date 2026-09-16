using System.Net;
using System.Text.Json;

namespace CreditFlow.Web.Core.Http;

internal static class ApiErrorParser
{
    public static async Task<ApiResult> ParseAsync(
        HttpResponseMessage response,
        string? fallbackMessage,
        CancellationToken cancellationToken)
    {
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var (message, code) = ParseContent(content);

        message ??= GetStatusMessage(response.StatusCode, fallbackMessage);
        return ApiResult.Failure(message, response.StatusCode, code);
    }

    private static (string? Message, string? Code) ParseContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return (null, null);

        try
        {
            using var document = JsonDocument.Parse(content);
            var root = document.RootElement;

            if (root.ValueKind == JsonValueKind.String)
                return (Normalize(root.GetString()), null);

            if (root.ValueKind != JsonValueKind.Object)
                return (null, null);

            var message = GetString(root, "mensaje")
                ?? GetString(root, "message")
                ?? GetString(root, "detail")
                ?? GetString(root, "error")
                ?? GetValidationMessage(root)
                ?? GetString(root, "title");

            var code = GetString(root, "codigo") ?? GetString(root, "code");
            return (Normalize(message), Normalize(code));
        }
        catch (JsonException)
        {
            return content.Length <= 500 && !content.Contains("Exception", StringComparison.OrdinalIgnoreCase)
                ? (Normalize(content.Trim('"')), null)
                : (null, null);
        }
    }

    private static string? GetString(JsonElement element, string name)
    {
        foreach (var property in element.EnumerateObject())
        {
            if (!property.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                continue;

            return property.Value.ValueKind == JsonValueKind.String
                ? property.Value.GetString()
                : null;
        }

        return null;
    }

    private static string? GetValidationMessage(JsonElement element)
    {
        foreach (var property in element.EnumerateObject())
        {
            if (!property.Name.Equals("errors", StringComparison.OrdinalIgnoreCase)
                || property.Value.ValueKind != JsonValueKind.Object)
                continue;

            foreach (var field in property.Value.EnumerateObject())
            {
                if (field.Value.ValueKind != JsonValueKind.Array)
                    continue;

                foreach (var error in field.Value.EnumerateArray())
                {
                    if (error.ValueKind == JsonValueKind.String)
                        return error.GetString();
                }
            }
        }

        return null;
    }

    private static string GetStatusMessage(HttpStatusCode statusCode, string? fallbackMessage)
    {
        return statusCode switch
        {
            HttpStatusCode.Unauthorized => "Tu sesión no es válida o ha expirado. Iniciá sesión nuevamente.",
            HttpStatusCode.Forbidden => "No tenés permiso para realizar esta acción.",
            HttpStatusCode.NotFound => fallbackMessage ?? "No se encontró la información solicitada.",
            HttpStatusCode.Conflict => fallbackMessage ?? "La operación entra en conflicto con el estado actual.",
            HttpStatusCode.UnprocessableEntity => fallbackMessage ?? "Los datos enviados no son válidos.",
            >= HttpStatusCode.InternalServerError => "El servidor no pudo procesar la solicitud. Intentá nuevamente más tarde.",
            _ => fallbackMessage ?? "No se pudo completar la solicitud."
        };
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
