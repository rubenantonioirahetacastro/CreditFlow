using CreditFlow.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Verification.Web.GetCreditRequests;

public sealed class ObtenerBandejaVerificacionHandler(
    DbNegocioContext context) : IObtenerBandejaVerificacionHandler
{
    private static readonly int[] EstadosVisibles = [1, 2, 3, 5];

    public async Task<IReadOnlyList<BandejaVerificacionItemDto>> EjecutarAsync(int? nCodAge)
    {
        var creditos = context.Creditos
            .AsNoTracking()
            .Where(credito => EstadosVisibles.Contains(credito.NEstado));

        if (nCodAge.HasValue)
            creditos = creditos.Where(credito => credito.NCodAge == nCodAge.Value);

        var items = await (
            from credito in creditos
            join persona in context.Personas.AsNoTracking()
                on credito.IdPersona equals persona.IdPersona into personas
            from persona in personas.DefaultIfEmpty()
            join agencia in context.Agencias.AsNoTracking()
                on credito.NCodAge equals agencia.NCodAge into agencias
            from agencia in agencias.DefaultIfEmpty()
            join estado in context.CatalogoCodigos.AsNoTracking()
                on new { Codigo = 116, Valor = credito.NEstado }
                equals new { Codigo = estado.NCodigo, Valor = estado.NValor } into estados
            from estado in estados.DefaultIfEmpty()
            join subProducto in context.CatalogoCodigos.AsNoTracking()
                on new { Codigo = 109, Valor = (int?)credito.NSubProd }
                equals new { Codigo = subProducto.NCodigo, Valor = (int?)subProducto.NValor }
                into subProductos
            from subProducto in subProductos.DefaultIfEmpty()
            select new BandejaVerificacionItemDto
            {
                NCodCred = credito.NCodCred,
                NCodAge = credito.NCodAge,
                Agencia = agencia == null ? null : agencia.CNomAge,
                NombreCliente = persona == null
                    ? null
                    : (persona.CNombres + " " + persona.CPrimerApellido + " " +
                        (persona.CSegundoApellido ?? "")).Trim(),
                IdPersona = persona == null ? null : persona.IdPersona,
                FotoUrl = persona == null ? null : (context.UsuarioLogins
                    .Where(login => login.IdUsuario == persona.IdUsuario)
                    .Select(login => login.VFoto)
                    .FirstOrDefault() ?? persona.VFotoPerfil),
                UsuarioGestion = persona == null ? null : persona.CUsuarioGestion,
                MontoSolicitado = credito.NPrestamo,
                DFecVig = credito.DFecVig,
                NEstado = credito.NEstado,
                Estado = estado == null ? null : estado.CNomCod,
                NSubProd = credito.NSubProd,
                SubProducto = subProducto == null ? null : subProducto.CNomCod
            }).ToListAsync();

        foreach (var item in items.Where(item => item.IdPersona.HasValue))
        {
            item.ConRepretamo = await EsRepretamoAsync(
                item.IdPersona!.Value,
                item.NCodAge,
                item.NCodCred);
        }

        return items;
    }

    private async Task<bool> EsRepretamoAsync(
        int idPersona,
        int nCodAgeActual,
        int nCodCredActual)
    {
        var creditoAnterior = await context.Creditos
            .AsNoTracking()
            .Where(credito =>
                credito.IdPersona == idPersona &&
                !(credito.NCodAge == nCodAgeActual && credito.NCodCred == nCodCredActual))
            .OrderByDescending(credito => credito.DFecVig)
            .FirstOrDefaultAsync();

        if (creditoAnterior == null || creditoAnterior.NPrestamo <= 0)
            return false;

        var numeroCalendario = creditoAnterior.IdCredCalendCond.HasValue
            ? await context.CredCalendConds
                .AsNoTracking()
                .Where(item => item.IdCredCalendCond == creditoAnterior.IdCredCalendCond.Value)
                .Select(item => (int?)item.NNroCalen)
                .FirstOrDefaultAsync()
            : null;

        var capitalPagado = await context.CredCalendarios
            .AsNoTracking()
            .Where(item =>
                item.NCodAge == creditoAnterior.NCodAge &&
                item.NCodCred == creditoAnterior.NCodCred &&
                (!numeroCalendario.HasValue || item.NNroCalen == numeroCalendario.Value))
            .SumAsync(item => item.NCapPag);

        return capitalPagado / creditoAnterior.NPrestamo >= 0.5m;
    }
}
