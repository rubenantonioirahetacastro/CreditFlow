namespace CreditFlow.API.Core.Security;

public static class AuthorizationPolicies
{
    public const string MobileEmployee = "MobileEmployee";
    public const string MobileVerifier = "MobileVerifier";
    public const string Maintenance = "Maintenance";
    public const string Administration = "Administration";
    public const string CalendarConfiguration = "CalendarConfiguration";
    public const string RoleIdClaim = CustomClaimTypes.RoleId;
}
