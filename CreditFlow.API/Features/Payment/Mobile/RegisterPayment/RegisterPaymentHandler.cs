using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Core.Errors;
using CreditFlow.API.Features.Payment.Shared.Errors;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Payment.Mobile.RegisterPayment;

public class RegisterPaymentHandler(DbNegocioContext context) : IRegisterPaymentHandler
{
    public async Task<List<CredCalendario>> ExecuteAsync(
        RegisterPaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.MontoAbonado <= 0)
            throw new RequestValidationException(PaymentErrors.InvalidAmount);

        var credit = await context.Creditos
            .FirstOrDefaultAsync(item =>
                item.NCodAge == request.NCodAge &&
                item.NCodCred == request.NCodCred)
            ?? throw new ResourceNotFoundException(PaymentErrors.CreditNotFound);

        if (credit.IdCredCalendCond == null)
            throw new ResourceConflictException(PaymentErrors.CalendarNotAssigned);

        var calendarCondition = await context.CredCalendConds
            .FirstOrDefaultAsync(item => item.IdCredCalendCond == credit.IdCredCalendCond.Value)
            ?? throw new ResourceNotFoundException(PaymentErrors.CalendarNotFound);

        var currentCalendarNumber = calendarCondition.NNroCalen;
        var pendingInstallments = await context.CredCalendarios
            .Where(item =>
                item.NCodAge == request.NCodAge &&
                item.NCodCred == request.NCodCred &&
                item.NNroCalen == currentCalendarNumber &&
                item.NEstado != 1)
            .OrderBy(item => item.NNroCuota)
            .ToListAsync();

        if (!pendingInstallments.Any())
            throw new BusinessRuleException(PaymentErrors.NoPendingInstallments);

        var creditLine = await context.CredLineaCreditos
            .FirstOrDefaultAsync(item => item.NCodLinea == credit.NCodLinea);
        var monthlyInterestRate = creditLine != null ? creditLine.NTasaCom : 0m;

        var remainingAmount = request.MontoAbonado;
        var paymentDate = DateTime.Now;
        var paidInstallments = new List<CredCalendario>();

        var currentAndOverdueInstallments = pendingInstallments
            .Where(item => item.DFecVenc.Date <= paymentDate.Date)
            .ToList();

        if (!currentAndOverdueInstallments.Any())
            currentAndOverdueInstallments.Add(pendingInstallments.First());

        foreach (var installment in currentAndOverdueInstallments)
        {
            if (remainingAmount <= 0)
                break;

            var remainingLateInterest = Math.Max(0, installment.NIntMor - installment.NIntMorPag);
            var remainingTax = Math.Max(0, installment.NIgv - installment.NIgvPag);
            var remainingInterest = Math.Max(0, installment.NIntComp - installment.NIntPag);
            var remainingPrincipal = Math.Max(0, installment.NCapital - installment.NCapPag);

            var paidLateInterest = Math.Min(remainingAmount, remainingLateInterest);
            installment.NIntMorPag += paidLateInterest;
            remainingAmount -= paidLateInterest;

            var paidTax = Math.Min(remainingAmount, remainingTax);
            installment.NIgvPag += paidTax;
            remainingAmount -= paidTax;

            var paidInterest = Math.Min(remainingAmount, remainingInterest);
            installment.NIntPag += paidInterest;
            remainingAmount -= paidInterest;

            var paidPrincipal = Math.Min(remainingAmount, remainingPrincipal);
            installment.NCapPag += paidPrincipal;
            remainingAmount -= paidPrincipal;
            credit.NSaldoK -= paidPrincipal;

            installment.DFecPago = paymentDate;
            installment.NEstado =
                installment.NCapital <= installment.NCapPag &&
                installment.NIntComp <= installment.NIntPag &&
                installment.NIgv <= installment.NIgvPag &&
                installment.NIntMor <= installment.NIntMorPag
                    ? 1
                    : 2;

            paidInstallments.Add(installment);
        }

        var advanceInstallments = pendingInstallments
            .Except(currentAndOverdueInstallments)
            .OrderBy(item => item.NNroCuota)
            .ToList();

        if (remainingAmount > 0 && advanceInstallments.Any())
        {
            foreach (var installment in advanceInstallments)
            {
                if (remainingAmount <= 0)
                    break;

                var remainingTax = Math.Max(0, installment.NIgv - installment.NIgvPag);
                var remainingPrincipal = Math.Max(0, installment.NCapital - installment.NCapPag);

                var paidTax = Math.Min(remainingAmount, remainingTax);
                installment.NIgvPag += paidTax;
                remainingAmount -= paidTax;

                var paidPrincipal = Math.Min(remainingAmount, remainingPrincipal);
                installment.NCapPag += paidPrincipal;
                remainingAmount -= paidPrincipal;

                credit.NSaldoK -= paidPrincipal;
                if (credit.NSaldoK < 0)
                    credit.NSaldoK = 0;

                installment.DFecPago = paymentDate;

                if (installment.NCapital <= installment.NCapPag && installment.NIgv <= installment.NIgvPag)
                {
                    installment.NEstado = 1;
                    installment.NIntComp = installment.NIntPag;
                    installment.NIntMor = installment.NIntMorPag;
                }
                else
                {
                    installment.NEstado = 0;
                }

                if (!paidInstallments.Contains(installment))
                    paidInstallments.Add(installment);
            }

            if (remainingAmount > 0)
            {
                credit.NSaldoK -= remainingAmount;
                if (credit.NSaldoK < 0)
                    credit.NSaldoK = 0;
                remainingAmount = 0;
            }
        }
        else if (remainingAmount > 0 && credit.NSaldoK > 0)
        {
            credit.NSaldoK -= remainingAmount;
            remainingAmount = 0;
            if (credit.NSaldoK < 0)
                credit.NSaldoK = 0;
        }

        if (credit.NSaldoK <= 0)
        {
            credit.NEstado = 1;
            foreach (var futureInstallment in pendingInstallments.Where(item => item.NEstado != 1))
            {
                if (paidInstallments.Contains(futureInstallment))
                    continue;

                futureInstallment.NCapital = 0;
                futureInstallment.NIntComp = 0;
                futureInstallment.NIgv = 0;
                futureInstallment.NTotalCuota = 0;
                futureInstallment.NEstado = 1;
            }
        }

        await context.SaveChangesAsync(cancellationToken);
        return paidInstallments;
    }
}
