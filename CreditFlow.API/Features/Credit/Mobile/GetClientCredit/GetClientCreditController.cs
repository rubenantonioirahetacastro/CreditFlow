using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Credit.Mobile.GetClientCredit;

[Route("api/Credito")]
[ApiController]
public sealed class GetClientCreditController(DbNegocioContext context) : ControllerBase
{
    [HttpGet("Credito/Persona/{idPersona}")]
    public async Task<IActionResult> Get(int idPersona)
    {
            var credit = await context.Creditos
                .Where(item => item.IdPersona == idPersona)
                .OrderByDescending(item => item.DFecVig)
                .ThenByDescending(item => item.NCodCred)
                .FirstOrDefaultAsync();

            if (credit == null)
                return NotFound(new { Mensaje = "No se encontró un crédito para la persona indicada." });

            return Ok(await BuildResponseAsync(credit));
    }

    private async Task<object> BuildResponseAsync(Credito credit)
    {
        var person = credit.IdPersona.HasValue
            ? await context.Personas.FirstOrDefaultAsync(item => item.IdPersona == credit.IdPersona.Value)
            : null;
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
        var pendingInstallments = calendar.Count(item => item.NEstado == 0);
        var nextInstallment = calendar
            .Where(item => item.NEstado == 0)
            .OrderBy(item => item.NNroCuota)
            .FirstOrDefault();
        var status = await context.CatalogoCodigos
            .Where(item => item.NCodigo == 116 && item.NValor == credit.NEstado)
            .Select(item => item.CNomCod)
            .FirstOrDefaultAsync();

        return new
        {
            nCodAge = credit.NCodAge,
            nCodCred = credit.NCodCred,
            EstadoCredito = status,
            numeroReferencia = $"{credit.NCodAge}-{credit.NCodCred}",
            nNroCalen = calendarCondition?.NNroCalen,
            nombreCliente = person == null
                ? null
                : $"{person.CNombres} {person.CPrimerApellido} {person.CSegundoApellido}".Trim(),
            nNroCuotasPendientes = pendingInstallments,
            cuotaProxima = nextInstallment == null ? 0m : nextInstallment.NTotalCuota ?? 0m,
            fechaVencimientoProxima = nextInstallment?.DFecVenc,
            numeroCuotaProxima = nextInstallment?.NNroCuota,
            saldoActual = credit.NSaldoK,
            calendarioActual = calendarCondition?.NNroCalen
        };
    }
}
