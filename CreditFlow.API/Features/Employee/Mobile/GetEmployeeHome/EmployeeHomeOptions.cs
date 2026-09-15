namespace CreditFlow.API.Features.Employee.Mobile.GetEmployeeHome;

public sealed class EmployeeHomeOptions
{
    public const string SectionName = "EmployeeHome";

    public int VerifierDailyGoal { get; init; } = 10;
    public string TimeZoneId { get; init; } = "America/El_Salvador";
}
