using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Features.Catalog.Shared;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Credit.Mobile.GetLastCredit;

[Route("api/Credito")]
[ApiController]
public sealed class GetLastCreditController(
    DbNegocioContext context,
    ICatalogRepository catalogRepository) : ControllerBase
{
    [HttpGet("Credito/Persona/{nIdPersona}/Ultimo")]
    public async Task<IActionResult> Get(int nIdPersona)
    {
            var credit = await context.Creditos
                .Where(item => item.IdPersona == nIdPersona)
                .OrderByDescending(item => item.DFecVig)
                .ThenByDescending(item => item.NCodCred)
                .FirstOrDefaultAsync();

            if (credit == null)
                return NotFound(new { Mensaje = "No se encontró un crédito para la persona indicada." });

            return Ok(await BuildResponseAsync(credit));
    }

    private async Task<LastCreditResponse> BuildResponseAsync(Credito credit)
    {
        var calendarCondition = credit.IdCredCalendCond.HasValue
            ? await context.CredCalendConds.FirstOrDefaultAsync(item =>
                item.IdCredCalendCond == credit.IdCredCalendCond.Value)
            : null;
        var calendar = await context.CredCalendarios
            .Where(item =>
                item.NCodAge == credit.NCodAge &&
                item.NCodCred == credit.NCodCred &&
                (calendarCondition == null || item.NNroCalen == calendarCondition.NNroCalen))
            .OrderBy(item => item.NNroCuota)
            .ToListAsync();
        var creditStatuses = await catalogRepository.GetByCodeAsync(116);
        var statusDescription = creditStatuses
            .FirstOrDefault(item => item.NValor == credit.NEstado)
            ?.CNomCod;
        var installment = calendar.FirstOrDefault()?.NTotalCuota ?? 0m;
        var dueDate = calendar.Count == 0
            ? (DateTime?)null
            : calendar.Max(item => item.DFecVenc);
        var paidCapital = calendar.Sum(item => item.NCapPag);
        var overdueInstallments = calendar.Count(item =>
            item.NEstado != 1 && item.DFecVenc.Date < DateTime.Today.Date);
        var overduePercentage = credit.NNroCuotas > 0
            ? (decimal)overdueInstallments / credit.NNroCuotas
            : 0m;

        if (!HasVisibleDueDate(credit.NEstado, statusDescription))
            dueDate = null;

        var paid = calendar.Sum(item =>
            item.NCapPag + item.NIntPag + item.NIntMorPag + item.NIgvPag);
        var pending = calendar.Sum(item => Math.Max(
            0m,
            (item.NTotalCuota ?? 0m) -
            (item.NCapPag + item.NIntPag + item.NIntMorPag + item.NIgvPag)));
        var paidInstallments = calendar.Count(item => item.NEstado == 1);
        var remainingInstallments = Math.Max(credit.NNroCuotas - paidInstallments, 0);

        return new LastCreditResponse
        {
            Ultimocred = new LastCreditDetailResponse
            {
                Prestamo = credit.NPrestamo,
                NroCuota = installment,
                Ncuotas = credit.NNroCuotas,
                FechaVencimiento = dueDate,
                Ncodcred = credit.NCodCred,
                BReprestamo = credit.NPrestamo > 0 && paidCapital / credit.NPrestamo >= 0.5m,
                BRefinanciamiento = overduePercentage > 0.30m
            },
            Resumenultcred = new LastCreditSummaryResponse
            {
                Pagado = paid,
                Pendiente = pending,
                CuotasRestantes = remainingInstallments,
                CuotaslTotales = credit.NNroCuotas,
                CuotasPagadas = paidInstallments
            }
        };
    }

    private static bool HasVisibleDueDate(int status, string? statusDescription)
    {
        if (status <= 30)
            return true;

        if (string.IsNullOrWhiteSpace(statusDescription))
            return false;

        return statusDescription.Contains("activo", StringComparison.OrdinalIgnoreCase) ||
               statusDescription.Contains("desembols", StringComparison.OrdinalIgnoreCase);
    }
}
