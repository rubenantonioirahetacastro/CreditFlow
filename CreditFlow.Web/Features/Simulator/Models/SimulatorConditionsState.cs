namespace CreditFlow.Web.Features.Simulator.Models;

public sealed class SimulatorConditionsState
{
    public int AgencyCode { get; set; }
    public int ProductCode { get; set; } = 4;
    public decimal Amount { get; set; } = 500m;
    public int Installments { get; set; } = 12;
    public int SubProductCode { get; set; } = 4;
    public DateTime StartDate { get; set; } = DateTime.Today;
    public bool AllowsSaturday { get; set; }
    public bool AllowsSunday { get; set; }
    public bool AllowsHoliday { get; set; }
    public int CategoryCode { get; set; } = 1;
    public bool IsRefinanced { get; set; }
    public string? RateOverride { get; set; }
    public string? ExpenseOverride { get; set; }
}

public sealed record SimulatorPeriodicityOption(string Label, int SubProductCode);

public static class SimulatorOptions
{
    public static IReadOnlyList<SimulatorPeriodicityOption> Periodicities { get; } =
    [
        new("Diario", 1),
        new("Semanal", 2),
        new("Quincenal", 3),
        new("Mensual", 4),
    ];
}
