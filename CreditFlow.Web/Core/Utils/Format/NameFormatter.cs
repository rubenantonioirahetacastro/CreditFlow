using System.Globalization;

namespace CreditFlow.Web.Core.Utils.Format;

public static class NameFormatter
{
    private static readonly CultureInfo Spanish = CultureInfo.GetCultureInfo("es-SV");

    /// <summary>
    /// Nombre de persona con solo la inicial de cada palabra en mayúscula ("JUAN PÉREZ" → "Juan Pérez"),
    /// sin espacios repetidos. Devuelve cadena vacía si no hay nombre.
    /// </summary>
    public static string ToDisplayName(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var words = value.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return Spanish.TextInfo.ToTitleCase(string.Join(' ', words).ToLower(Spanish));
    }
}
