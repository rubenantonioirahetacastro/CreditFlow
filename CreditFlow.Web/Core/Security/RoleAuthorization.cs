using System.Security.Claims;

namespace CreditFlow.Web.Core.Security;

public static class RoleAuthorization
{
    public static bool HasAnyRoleId(ClaimsPrincipal user, IReadOnlyCollection<int> allowedRoleIds)
    {
        return user.FindAll(CustomClaimTypes.RoleId)
            .Select(claim => int.TryParse(claim.Value, out var roleId) ? roleId : 0)
            .Any(allowedRoleIds.Contains);
    }
}
