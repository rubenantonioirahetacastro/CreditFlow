using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Credit.Shared.Calendar;

// Mirrors the expense-per-installment rules from
// [esv].[CredW_RecuperaGastosAlDesemb_2]. The period must already be expressed
// in days by the caller.
public class ExpenseService : IExpenseService
{
    private readonly DbNegocioContext _context;

    public ExpenseService(DbNegocioContext context)
    {
        _context = context;
    }

    public async Task<decimal> GetExpenseAsync(ExpenseRequest request)
    {
        if (request.CollectAtAgency == 1)
            return 0m;

        if (request.CreditCode > 0)
        {
            var changedAmount = await _context.CredGastoCuotaCambios
                .Where(change => change.NCodAge == request.AgencyCode
                    && change.NCodCred == request.CreditCode)
                .Select(change => (decimal?)change.NMontoNuevo)
                .FirstOrDefaultAsync();

            if (changedAmount.HasValue)
                return changedAmount.Value;
        }

        IQueryable<CredGasto> query = _context.CredGastos
            .Where(expense => expense.NRangoInicial <= request.Amount
                && expense.NRangoFinal >= request.Amount
                && expense.NMoneda == request.Currency
                && expense.NTipoValor == 1
                && expense.NProd == request.Product
                && expense.NSubProd == request.SubProduct
                && expense.NTipoGasto == 2
                && expense.NPeriodo == request.Period);

        if (request.SecondaryCreditLineCode > 0)
        {
            return await query
                .Where(expense => expense.NTipoCargo == request.ChargeType)
                .Select(expense => expense.NValor)
                .FirstOrDefaultAsync();
        }

        return await query
            .Where(expense => expense.NTipoCargo == 0 || expense.NTipoCargo == 1)
            .SumAsync(expense => (decimal?)expense.NValor) ?? 0m;
    }
}
