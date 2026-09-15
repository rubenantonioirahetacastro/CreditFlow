namespace CreditFlow.API.Features.Authentication.Shared.Session;

public sealed record UserSession(
    int UserId,
    string Document,
    int? PersonId,
    int? EmployeeId,
    int RoleId,
    IReadOnlyList<int> RoleIds);
