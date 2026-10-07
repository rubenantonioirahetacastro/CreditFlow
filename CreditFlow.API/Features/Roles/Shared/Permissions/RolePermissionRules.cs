using System.Text.RegularExpressions;
using CreditFlow.API.Core.Security;

namespace CreditFlow.API.Features.Roles.Shared.Permissions;

public static partial class RolePermissionRules
{
    /// <summary>Clave de la opción «Permisos por rol» de la Web.</summary>
    public const string RolePermissionsMenuKey = "mantenimientos.permisos";

    public const int MaxKeyLength = 100;

    /// <summary>
    /// Administrador y Tecnología siempre conservan acceso total a «Permisos por rol», para que nadie
    /// pueda quedar fuera del mantenimiento quitándose el permiso por error.
    /// </summary>
    public static bool AlwaysManagesPermissions(int roleId) => RoleIds.GlobalAdministrators.Contains(roleId);

    public static RolePermissionDto FullAccess(string key) => new()
    {
        Clave = key,
        Ver = true,
        Crear = true,
        Editar = true,
        Eliminar = true
    };

    /// <summary>Minúsculas, números, «.» y «-»; p. ej. «otorgamiento.tesoreria.abonar-agencia».</summary>
    public static bool IsValidKey(string? key) =>
        !string.IsNullOrWhiteSpace(key) && key.Length <= MaxKeyLength && KeyPattern().IsMatch(key);

    [GeneratedRegex("^[a-z0-9]+([.-][a-z0-9]+)*$")]
    private static partial Regex KeyPattern();
}
