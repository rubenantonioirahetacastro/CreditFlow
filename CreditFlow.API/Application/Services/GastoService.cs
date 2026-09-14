using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Infrastructure.Data;
using CreditFlow.API.Application.Interfaces;
using CreditFlow.API.Application.Requests;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Application.Services
{
    // Replica exacta de [esv].[CredW_RecuperaGastosAlDesemb_2] (gasto por cuota,
    // clsCredProcesos_SV.ObtieneGastoPorCuotaTotalCal_2). El nPeriodo debe venir ya
    // resuelto por el llamador (días del período: 30 Mensual, 7 Semanal, 15
    // Quincenal, 14 Catorcenal, 1 Diario) — el SP solo aplica ese mapeo por defecto
    // para nProd IN (4,5,6); para el resto exige el período explícito.
    public class GastoService : IGastoService
    {
        private readonly DbNegocioContext _context;

        public GastoService(DbNegocioContext context)
        {
            _context = context;
        }

        public async Task<decimal> ObtenerGastoAsync(CreditoRequest request)
        {
            if (request.nCobroEnAgencia == 1)
                return 0m;

            if (request.nCodCred > 0)
            {
                var cambio = await _context.CredGastoCuotaCambios
                    .Where(c => c.NCodAge == request.nCodAge && c.NCodCred == request.nCodCred)
                    .Select(c => (decimal?)c.NMontoNuevo)
                    .FirstOrDefaultAsync();

                if (cambio.HasValue)
                    return cambio.Value;
            }

            IQueryable<CredGasto> query = _context.CredGastos
                .Where(g => g.NRangoInicial <= request.nPrestamo
                    && g.NRangoFinal >= request.nPrestamo
                    && g.NMoneda == request.nMoneda
                    && g.NTipoValor == 1
                    && g.NProd == request.nProd
                    && g.NSubProd == request.nSubProd
                    && g.NTipoGasto == 2
                    && g.NPeriodo == request.nPeriodo);

            if (request.nCodLineaSecundario > 0)
            {
                return await query
                    .Where(g => g.NTipoCargo == request.nTipoCargo)
                    .Select(g => g.NValor)
                    .FirstOrDefaultAsync();
            }

            return await query
                .Where(g => g.NTipoCargo == 0 || g.NTipoCargo == 1)
                .SumAsync(g => (decimal?)g.NValor) ?? 0m;
        }
    }
}
