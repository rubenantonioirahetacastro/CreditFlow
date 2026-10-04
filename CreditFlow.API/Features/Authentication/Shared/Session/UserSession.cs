using CreditFlow.API.Core.Security;

namespace CreditFlow.API.Features.Authentication.Shared.Session;

public sealed record UserSession(
    int UserId,
    string Document,
    int? PersonId,
    int? EmployeeId,
    int RoleId,
    IReadOnlyList<int> RoleIds)
{
    public IReadOnlyList<string> Capabilities => RoleCapabilities.For(RoleIds);
}
