using System.Security.Claims;

namespace CreditFlow.Web.Core.Security;

/// <summary>
/// Etiquetas visibles de los roles. Solo para mostrar: la autorización siempre compara IDs.
/// </summary>
public static class RoleDisplayNames
{
    public static string For(int roleId) => roleId switch
    {
        RoleIds.Administrator => "Administrador",
        RoleIds.Client => "Cliente",
        RoleIds.Supervisor => "Supervisor",
        RoleIds.CreditOfficer => "Oficial de crédito",
        RoleIds.Cashier => "Cajero",
        RoleIds.CreditDisbursementOfficer => "Oficial de desembolso",
        RoleIds.Technology => "Tecnología",
        _ => "Usuario"
    };

    /// <summary>Etiqueta del primer rol válido del usuario autenticado.</summary>
    public static string ForUser(ClaimsPrincipal user)
    {
        var roleId = user.FindAll(CustomClaimTypes.RoleId)
            .Select(claim => int.TryParse(claim.Value, out var id) ? id : 0)
            .FirstOrDefault(id => id > 0);
        return For(roleId);
    }
}
