using System.Globalization;

namespace CreditFlow.Web.Features.Maintenance.Catalog.Validation;

/// <summary>Errores por campo para la validación en vivo de los formularios de catálogos.</summary>
public sealed record CatalogFormErrors(string? Numero, string? Nombre)
{
    public bool HayErrores => Numero is not null || Nombre is not null;
}

public static class CatalogValidator
{
    /// <summary>Valor de un catálogo: numérico, positivo, no repetido en el catálogo (ni igual al código) y con nombre.</summary>
    public static CatalogFormErrors ValidarValor(
        string valorTexto,
        string nombre,
        int codigoCatalogo,
        IEnumerable<int> valoresExistentes,
        bool esNuevo)
    {
        string? errorValor = null;
        if (esNuevo)
        {
            if (!int.TryParse(valorTexto, NumberStyles.None, CultureInfo.InvariantCulture, out var valor) || valor <= 0)
                errorValor = "El valor debe ser un número entero mayor a cero.";
            else if (valor == codigoCatalogo)
                errorValor = "Ese número está reservado para el encabezado del catálogo.";
            else if (valoresExistentes.Contains(valor))
                errorValor = $"El valor {valor} ya existe en este catálogo.";
        }

        var errorNombre = string.IsNullOrWhiteSpace(nombre) ? "El nombre es obligatorio." : null;
        return new CatalogFormErrors(errorValor, errorNombre);
    }

    /// <summary>Catálogo nuevo: código numérico, positivo y único, y nombre visible obligatorio.</summary>
    public static CatalogFormErrors ValidarCatalogo(string codigoTexto, string nombre, IEnumerable<int> codigosExistentes)
    {
        string? errorCodigo = null;
        if (!int.TryParse(codigoTexto, NumberStyles.None, CultureInfo.InvariantCulture, out var codigo) || codigo <= 0)
            errorCodigo = "El código debe ser un número entero mayor a cero.";
        else if (codigosExistentes.Contains(codigo))
            errorCodigo = $"El código {codigo} ya está en uso.";

        var errorNombre = string.IsNullOrWhiteSpace(nombre) ? "El nombre es obligatorio." : null;
        return new CatalogFormErrors(errorCodigo, errorNombre);
    }
}
