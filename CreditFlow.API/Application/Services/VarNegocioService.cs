using System.Globalization;
using CreditFlow.API.Application.Interfaces;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Application.Services
{
    public class VarNegocioService : IVarNegocioService
    {
        private readonly DbNegocioContext _context;

        public VarNegocioService(DbNegocioContext context)
        {
            _context = context;
        }

        public async Task<string?> ObtenerValorAsync(int nCodVar)
        {
            return await _context.VerNegocios
                .Where(v => v.NCodVar == nCodVar)
                .Select(v => v.CValorVar)
                .FirstOrDefaultAsync();
        }

        public async Task<int> ObtenerValorIntAsync(int nCodVar, int valorPorDefecto)
        {
            var valor = await ObtenerValorAsync(nCodVar);
            return int.TryParse(valor, NumberStyles.Integer, CultureInfo.InvariantCulture, out var resultado)
                ? resultado
                : valorPorDefecto;
        }

        public async Task<decimal> ObtenerValorDecimalAsync(int nCodVar, decimal valorPorDefecto)
        {
            var valor = await ObtenerValorAsync(nCodVar);
            return decimal.TryParse(valor, NumberStyles.Number, CultureInfo.InvariantCulture, out var resultado)
                ? resultado
                : valorPorDefecto;
        }
    }
}
