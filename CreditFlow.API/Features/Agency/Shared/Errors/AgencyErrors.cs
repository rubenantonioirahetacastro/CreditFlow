using CreditFlow.API.Core.Errors;

namespace CreditFlow.API.Features.Agency.Shared.Errors;

public static class AgencyErrors
{
    public static readonly ErrorDefinition NotFound = new(
        "agency_not_found",
        "Agencia no encontrada.");

    public static ErrorDefinition AlreadyExists(int agencyCode) => new(
        "agency_already_exists",
        $"Ya existe una agencia con el código {agencyCode}.");
}
