namespace CreditFlow.Web.Core.Security;

public static class RoleIds
{
    public const int Administrator = 1;
    public const int Client = 2;
    public const int Supervisor = 3;
    public const int CreditOfficer = 4;
    public const int Cashier = 5;
    public const int CreditDisbursementOfficer = 6;
    public const int Technology = 7;

    public static readonly int[] GlobalAdministrators = [Administrator, Technology];
    public static readonly int[] MaintenanceAccess = [Administrator, Supervisor, Technology];
    public static readonly int[] MaintenanceEditAccess = [Administrator, Technology];
    public static readonly int[] CalendarConfigurationAccess = [Administrator, Supervisor, CreditOfficer, Technology];
}
