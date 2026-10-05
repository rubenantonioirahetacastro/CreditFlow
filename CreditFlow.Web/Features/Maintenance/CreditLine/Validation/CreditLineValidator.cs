using CreditFlow.Web.Features.Maintenance.CreditLine.Models;

namespace CreditFlow.Web.Features.Maintenance.CreditLine.Validation;

/// <summary>Errores por campo para la validación en vivo del formulario de línea de crédito.</summary>
public sealed record CreditLineFormErrors(
    string? Descripcion,
    string? Producto,
    string? SubProducto,
    string? Tasa,
    string? Plazo,
    string? Monto,
    string? Prestamos)
{
    public bool HayErrores =>
        Descripcion is not null || Producto is not null || SubProducto is not null || Tasa is not null ||
        Plazo is not null || Monto is not null || Prestamos is not null;
}

public static class CreditLineValidator
{
    public const int MaxDescriptionLength = 150;

    public static CreditLineFormErrors Validate(LineaCreditoFormulario linea)
    {
        string? descripcion = null;
        if (string.IsNullOrWhiteSpace(linea.Descripcion))
            descripcion = "La descripción de la línea es obligatoria.";
        else if (linea.Descripcion.Trim().Length > MaxDescriptionLength)
            descripcion = $"La descripción no puede superar los {MaxDescriptionLength} caracteres.";

        var tasa = LineaCreditoFormulario.Decimal(linea.TasaComision);

        return new CreditLineFormErrors(
            descripcion,
            LineaCreditoFormulario.Entero(linea.Producto) is > 0 ? null : "Indica el código de producto (número mayor a cero).",
            LineaCreditoFormulario.Entero(linea.SubProducto) is > 0 ? null : "Indica el código de subproducto (número mayor a cero).",
            tasa is null ? "La tasa debe ser un número igual o mayor a cero." : null,
            Rango(LineaCreditoFormulario.Entero(linea.PlazoMinimo), LineaCreditoFormulario.Entero(linea.PlazoMaximo), "plazo", obligatorio: true),
            Rango(LineaCreditoFormulario.Decimal(linea.MontoMinimo), LineaCreditoFormulario.Decimal(linea.MontoMaximo), "monto", obligatorio: true),
            RangoOpcional(linea.PrestamosMinimo, linea.PrestamosMaximo));
    }

    private static string? Rango<T>(T? minimo, T? maximo, string nombre, bool obligatorio) where T : struct, IComparable<T>
    {
        if (minimo is null || maximo is null)
            return obligatorio ? $"Indica el {nombre} mínimo y máximo (números mayores a cero)." : null;

        if (minimo.Value.CompareTo(default) <= 0 || maximo.Value.CompareTo(default) <= 0)
            return $"El {nombre} mínimo y máximo deben ser mayores a cero.";

        return minimo.Value.CompareTo(maximo.Value) > 0
            ? $"El {nombre} mínimo no puede ser mayor al máximo."
            : null;
    }

    private static string? RangoOpcional(string minimoTexto, string maximoTexto)
    {
        var minimo = LineaCreditoFormulario.Entero(minimoTexto);
        var maximo = LineaCreditoFormulario.Entero(maximoTexto);

        if ((!string.IsNullOrWhiteSpace(minimoTexto) && minimo is null) ||
            (!string.IsNullOrWhiteSpace(maximoTexto) && maximo is null))
            return "El número de préstamos debe ser un entero.";

        return minimo is not null && maximo is not null && minimo > maximo
            ? "El número mínimo de préstamos no puede ser mayor al máximo."
            : null;
    }
}
