using CreditFlow.API.Core.Errors;

namespace CreditFlow.API.Features.Authentication.Shared.Errors;

public static class AuthenticationErrors
{
    public static readonly ErrorDefinition CredentialsRequired = new(
        "auth_credentials_required",
        "El documento y la contraseña son obligatorios.");

    public static readonly ErrorDefinition InvalidCredentials = new(
        "auth_invalid_credentials",
        "Usuario o contraseña inválidos.");

    public static readonly ErrorDefinition UserBlocked = new(
        "auth_user_blocked",
        "Usuario bloqueado.");

    public static readonly ErrorDefinition RoleNotAllowed = new(
        "auth_role_not_allowed",
        "Usuario no tiene rol asignado o rol no permitido.");

    public static readonly ErrorDefinition ActiveRoleRequired = new(
        "auth_active_role_required",
        "Usuario no tiene un rol activo asignado.");
}
