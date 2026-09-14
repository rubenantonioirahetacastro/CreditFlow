using CreditFlow.API.Application.DTOs;
using CreditFlow.API.Application.Interfaces;
using CreditFlow.API.Application.Requests;
using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Application.Services
{
    // Replica exacta de [ESV].[ObtenerPrimLineaCred] (clsCredProcesos_SV.ObtenerPrimLineaCredSV).
    public class PrimLineaCreditoService : IPrimLineaCreditoService
    {
        private readonly DbNegocioContext _context;

        public PrimLineaCreditoService(DbNegocioContext context)
        {
            _context = context;
        }

        public async Task<PrimLineaCreditoResult> ObtenerPrimLineaCredAsync(PrimLineaCreditoRequest request)
        {
            bool bReadecuacion = false;
            if (request.NCodCred > 0)
            {
                bReadecuacion = await _context.Creditos
                    .Where(c => c.NCodAge == request.NCodAge && c.NCodCred == request.NCodCred)
                    .Select(c => (bool?)c.BReadecuacion)
                    .FirstOrDefaultAsync() ?? false;
            }

            var candidatas =
                from linea in _context.CredLineaCreditos.AsNoTracking()
                join age in _context.CredLineaCreditoAges.AsNoTracking() on linea.NCodLinea equals age.NCodLinea
                join camp in _context.CredLineaCreditoCamps.AsNoTracking() on linea.NCodLinea equals camp.NCodLinea
                where age.NCodAge == request.NCodAge
                    && camp.NCodCamp == request.NCodCamp
                    && linea.NPlazoMin <= request.NCuotas
                    && linea.NPlazoMax >= request.NCuotas
                    && linea.NMontoMin <= request.NMonto
                    && linea.NMontoMax >= request.NMonto
                    && linea.NMoneda == request.NMoneda
                    && linea.NProd == request.NProd
                    && linea.NSubProd == request.NSubProd
                    && (linea.NCategoria == request.NCategoria || linea.NCategoria == null || linea.NCategoria == 0)
                select linea;

            if (request.NProd == 4 && !bReadecuacion)
            {
                candidatas = candidatas.Where(l => l.BEstado && !l.BReadecuacion);
            }
            else if (request.NProd == 4 && request.NSubProd == 4 && bReadecuacion)
            {
                candidatas = candidatas.Where(l => !l.BEstado && l.BReadecuacion);
            }
            else
            {
                // Para el resto de productos (incluye Mensual, nProd=1) solo importa bEstado.
                candidatas = candidatas.Where(l => l.BEstado);
            }

            CredLineaCredito? linea1 = await candidatas
                .OrderByDescending(l => l.NTasaCom)
                .FirstOrDefaultAsync();

            decimal tasaComision = await _context.CredGastos
                .Where(g => g.NRangoInicial <= request.NMonto
                    && g.NRangoFinal >= request.NMonto
                    && (g.BRefinan ?? false) == request.BRefinanciado
                    && g.NMoneda == request.NMoneda
                    && g.NProd == request.NProd
                    && g.NSubProd == request.NSubProd
                    && g.NTipoValor == 2
                    && g.NTipoGasto == 3
                    && g.BCustodia == request.BCustodia
                    && (request.NPeriodo == -1 || g.NPeriodo == request.NPeriodo))
                .SumAsync(g => (decimal?)g.NValor) ?? 0m;

            return new PrimLineaCreditoResult(
                linea1?.NCodLinea ?? 0,
                linea1?.CDescripcion ?? string.Empty,
                linea1?.NTasaCom ?? 0m,
                tasaComision);
        }
    }
}
