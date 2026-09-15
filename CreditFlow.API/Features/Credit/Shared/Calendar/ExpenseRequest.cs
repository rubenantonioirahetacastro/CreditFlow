namespace CreditFlow.API.Features.Credit.Shared.Calendar;

public class ExpenseRequest
{
    public decimal Amount { get; set; }
    public int Currency { get; set; } = 1;
    public int Product { get; set; }
    public int SubProduct { get; set; }
    public bool IsRefinanced { get; set; }
    public int Period { get; set; }
    public int ChargeType { get; set; }
    public int SecondaryCreditLineCode { get; set; }
    public int CollectAtAgency { get; set; }
    public int CreditCode { get; set; }
    public int AgencyCode { get; set; }
    public DateTime DisbursementDate { get; set; }
}
