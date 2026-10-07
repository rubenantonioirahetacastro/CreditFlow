using CreditFlow.API.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Credit.Web.GetCredits;

[Route("api/Credito")]
[ApiController]
public sealed class GetCreditsController(DbNegocioContext context) : ControllerBase
{
    [HttpGet("listaCreditos-cpc")]
    public async Task<IActionResult> Get([FromQuery] int? ncodage)
    {
            var visibleStatuses = new[] { 1, 2, 3 };
            var credits = context.Creditos
                .Where(item => visibleStatuses.Contains(item.NEstado));

            if (ncodage.HasValue)
                credits = credits.Where(item => item.NCodAge == ncodage.Value);

            var result = await (
                from credit in credits
                join person in context.Personas
                    on credit.IdPersona equals person.IdPersona into people
                from person in people.DefaultIfEmpty()
                join agency in context.Agencias
                    on credit.NCodAge equals agency.NCodAge into agencies
                from agency in agencies.DefaultIfEmpty()
                join status in context.CatalogoCodigos
                    on new { Code = 116, Value = credit.NEstado }
                    equals new { Code = status.NCodigo, Value = status.NValor } into statuses
                from status in statuses.DefaultIfEmpty()
                join subProduct in context.CatalogoCodigos
                    on new { Code = 109, Value = (int?)credit.NSubProd }
                    equals new { Code = subProduct.NCodigo, Value = (int?)subProduct.NValor }
                    into subProducts
                from subProduct in subProducts.DefaultIfEmpty()
                select new CreditListResponse
                {
                    NCodCred = credit.NCodCred,
                    NCodAge = credit.NCodAge,
                    Agencia = agency == null ? null : agency.CNomAge,
                    MontoSolicitado = credit.NPrestamo,
                    DFecVig = credit.DFecVig,
                    NEstado = credit.NEstado,
                    Estado = status == null ? null : status.CNomCod,
                    NSubProd = credit.NSubProd,
                    SubProducto = subProduct == null ? null : subProduct.CNomCod,
                    NombreCliente = person == null
                        ? null
                        : (person.CNombres + " " + person.CPrimerApellido + " " +
                           (person.CSegundoApellido ?? "")).Trim(),
                    IdPersona = person == null ? null : person.IdPersona,
                    FotoUrl = person == null ? null : (context.UsuarioLogins
                        .Where(login => login.IdUsuario == person.IdUsuario)
                        .Select(login => login.VFoto)
                        .FirstOrDefault() ?? person.VFotoPerfil),
                    UsuarioGestion = person == null ? null : person.CUsuarioGestion
                }).ToListAsync();

            foreach (var item in result.Where(item => item.IdPersona.HasValue))
            {
                item.ConRepretamo = await IsRepeatCreditAsync(
                    item.IdPersona!.Value,
                    item.NCodAge,
                    item.NCodCred);
            }

            return Ok(result);
    }

    private async Task<bool> IsRepeatCreditAsync(
        int personId,
        int currentAgencyCode,
        int currentCreditCode)
    {
        var previousCredit = await context.Creditos
            .Where(item =>
                item.IdPersona == personId &&
                !(item.NCodAge == currentAgencyCode && item.NCodCred == currentCreditCode))
            .OrderByDescending(item => item.DFecVig)
            .FirstOrDefaultAsync();

        if (previousCredit == null || previousCredit.NPrestamo <= 0)
            return false;

        var calendarCondition = previousCredit.IdCredCalendCond.HasValue
            ? await context.CredCalendConds.FirstOrDefaultAsync(item =>
                item.IdCredCalendCond == previousCredit.IdCredCalendCond.Value)
            : null;

        var paidCapital = await context.CredCalendarios
            .Where(item =>
                item.NCodAge == previousCredit.NCodAge &&
                item.NCodCred == previousCredit.NCodCred &&
                (calendarCondition == null || item.NNroCalen == calendarCondition.NNroCalen))
            .SumAsync(item => item.NCapPag);

        return paidCapital / previousCredit.NPrestamo >= 0.5m;
    }
}
