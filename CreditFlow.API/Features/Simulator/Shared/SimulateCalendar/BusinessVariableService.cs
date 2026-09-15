using System.Globalization;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Simulator.Shared.SimulateCalendar
{
    public class BusinessVariableService : IBusinessVariableService
    {
        private readonly DbNegocioContext _context;

        public BusinessVariableService(DbNegocioContext context)
        {
            _context = context;
        }

        public async Task<string?> GetValueAsync(int variableCode)
        {
            return await _context.VerNegocios
                .Where(v => v.NCodVar == variableCode)
                .Select(v => v.CValorVar)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetIntAsync(int variableCode, int defaultValue)
        {
            var valor = await GetValueAsync(variableCode);
            return int.TryParse(valor, NumberStyles.Integer, CultureInfo.InvariantCulture, out var resultado)
                ? resultado
                : defaultValue;
        }

        public async Task<decimal> GetDecimalAsync(int variableCode, decimal defaultValue)
        {
            var valor = await GetValueAsync(variableCode);
            return decimal.TryParse(valor, NumberStyles.Number, CultureInfo.InvariantCulture, out var resultado)
                ? resultado
                : defaultValue;
        }
    }
}
