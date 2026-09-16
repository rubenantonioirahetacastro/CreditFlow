using CreditFlow.Web.Core.Validation;
using CreditFlow.Web.Features.Maintenance.CreditLine.Models;

namespace CreditFlow.Web.Features.Maintenance.CreditLine.Validation;

public static class CreditLineValidator
{
    private const int MaxDescriptionLength = 150;

    public static UiValidationResult Validate(LineaCreditoDto creditLine)
    {
        if (string.IsNullOrWhiteSpace(creditLine.Descripcion))
            return UiValidationResult.Failure("La descripción de la línea es obligatoria.");

        if (creditLine.Descripcion.Trim().Length > MaxDescriptionLength)
            return UiValidationResult.Failure($"La descripción no puede superar los {MaxDescriptionLength} caracteres.");

        if (creditLine.TasaComision < 0)
            return UiValidationResult.Failure("La tasa de comisión no puede ser negativa.");

        if (creditLine.Producto <= 0)
            return UiValidationResult.Failure("Debe seleccionar un producto.");

        if (creditLine.SubProducto <= 0)
            return UiValidationResult.Failure("Debe seleccionar un subproducto.");

        if (creditLine.PlazoMinimo <= 0 || creditLine.PlazoMaximo <= 0)
            return UiValidationResult.Failure("Los plazos deben ser mayores que cero.");

        if (creditLine.PlazoMinimo > creditLine.PlazoMaximo)
            return UiValidationResult.Failure("El plazo mínimo no puede ser mayor al plazo máximo.");

        if (creditLine.MontoMinimo <= 0 || creditLine.MontoMaximo <= 0)
            return UiValidationResult.Failure("Los montos deben ser mayores que cero.");

        if (creditLine.MontoMinimo > creditLine.MontoMaximo)
            return UiValidationResult.Failure("El monto mínimo no puede ser mayor al monto máximo.");

        return UiValidationResult.Success();
    }
}
