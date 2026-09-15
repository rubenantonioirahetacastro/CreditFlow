namespace CreditFlow.API.Features.Credit.Shared.Calendar
{
    public interface IExpenseService
    {
        Task<decimal> GetExpenseAsync(ExpenseRequest request);
    }
}
