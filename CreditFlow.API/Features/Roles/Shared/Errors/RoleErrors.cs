using CreditFlow.API.Core.Errors;

namespace CreditFlow.API.Features.Roles.Shared.Errors;

public static class RoleErrors
{
    public static readonly ErrorDefinition NameRequired = new(
        "role_name_required",
        "El nombre del rol es requerido.");

    public static readonly ErrorDefinition NotFound = new(
        "role_not_found",
        "El rol no existe.");

    public static ErrorDefinition ActiveNameAlreadyExists(string name) => new(
        "role_name_already_exists",
        $"Ya existe un rol activo con el nombre '{name}'.");
}
