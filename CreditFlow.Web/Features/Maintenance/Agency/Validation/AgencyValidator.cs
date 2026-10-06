using System.Globalization;
using CreditFlow.Web.Core.Validation;

namespace CreditFlow.Web.Features.Maintenance.Agency.Validation;

/// <summary>Errores por campo para la validación en vivo del formulario de agencia.</summary>
public sealed record AgencyFormErrors(string? Codigo, string? Nombre, string? Correo)
{
    public bool HayErrores => Codigo is not null || Nombre is not null || Correo is not null;
}

public static class AgencyValidator
{
    /// <summary>Código numérico, positivo y único (solo al crear), nombre obligatorio y correo con formato válido.</summary>
    public static AgencyFormErrors Validate(
        string codigoTexto,
        string nombre,
        string correo,
        IEnumerable<int> codigosExistentes,
        bool esNueva)
    {
        string? errorCodigo = null;
        if (esNueva)
        {
            if (!int.TryParse(codigoTexto.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out var codigo) || codigo <= 0)
                errorCodigo = "El código debe ser un número entero mayor a cero.";
            else if (codigosExistentes.Contains(codigo))
                errorCodigo = $"El código {codigo} ya está en uso.";
        }

        return new AgencyFormErrors(
            errorCodigo,
            string.IsNullOrWhiteSpace(nombre) ? "El nombre de la agencia es obligatorio." : null,
            EmailValidator.IsValid(correo) ? null : "El correo electrónico no es válido.");
    }
}
