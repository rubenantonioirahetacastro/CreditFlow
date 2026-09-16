using CreditFlow.Web.Core.Validation;
using CreditFlow.Web.Shared.Catalog.Models;

namespace CreditFlow.Web.Features.Maintenance.Catalog.Validation;

public static class CatalogValidator
{
    public static UiValidationResult Validate(CatalogoCodigoDto catalog, bool isNew)
    {
        if (isNew && catalog.NCodigo <= 0)
            return UiValidationResult.Failure("El código de catálogo debe ser mayor a cero.");

        if (string.IsNullOrWhiteSpace(catalog.CNomCod))
            return UiValidationResult.Failure("El nombre del valor de catálogo es obligatorio.");

        return UiValidationResult.Success();
    }
}
