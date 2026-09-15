namespace CreditFlow.API.Core.Security;

public static class RoleIds
{
    public const int Administrator = 1;
    public const int Client = 2;
    public const int Supervisor = 3;
    public const int CreditOfficer = 4;
    public const int Cashier = 5;
    public const int DisbursementOfficer = 6;
    public const int Technology = 7;

    public static readonly int[] GlobalAdministrators = [Administrator, Technology];
    public static readonly int[] MobileAccess = [Administrator, Client, Supervisor, Cashier, Technology];
    public static readonly int[] WebAccess = [Administrator, Supervisor, CreditOfficer, DisbursementOfficer, Technology];
    public static readonly int[] MaintenanceAccess = [Administrator, Supervisor, Technology];
    public static readonly int[] AdministrationAccess = [Administrator, Technology];
    public static readonly int[] VerificationAccess = [Administrator, Supervisor, Technology];
    public static readonly int[] CalendarConfigurationAccess = [Administrator, Supervisor, CreditOfficer, Technology];
}
