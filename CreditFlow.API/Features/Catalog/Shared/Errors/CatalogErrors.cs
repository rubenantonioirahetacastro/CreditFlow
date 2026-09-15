using CreditFlow.API.Core.Errors;

namespace CreditFlow.API.Features.Catalog.Shared.Errors;

public static class CatalogErrors
{
    public static readonly ErrorDefinition NotFound = new(
        "catalog_not_found",
        "El catálogo no existe.");

    public static ErrorDefinition AlreadyExists(int code, int value) => new(
        "catalog_value_already_exists",
        $"Ya existe el valor {value} para el catálogo {code}.");
}
