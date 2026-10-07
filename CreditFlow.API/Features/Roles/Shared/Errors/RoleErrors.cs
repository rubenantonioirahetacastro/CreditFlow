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

    public static ErrorDefinition InvalidMenuKey(string? key) => new(
        "role_permission_invalid_key",
        $"La opción de menú '{key}' no es válida.");

    public static readonly ErrorDefinition DuplicatedMenuKey = new(
        "role_permission_duplicated_key",
        "La misma opción de menú viene más de una vez.");

    public static ErrorDefinition ActiveNameAlreadyExists(string name) => new(
        "role_name_already_exists",
        $"Ya existe un rol activo con el nombre '{name}'.");
}
