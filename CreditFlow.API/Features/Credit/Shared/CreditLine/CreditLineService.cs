using CreditFlow.API.Core.Errors;
using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Features.Credit.Shared.Errors;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Credit.Shared.CreditLine
{
    public interface ILineaCreditoService
    {
        Task<CredLineaCredito> ResolverLineaCreditoAsync(int nSubProd, decimal montoSolicitado);
    }

    public class LineaCreditoService : ILineaCreditoService
    {
        private readonly DbNegocioContext _context;
        public LineaCreditoService(DbNegocioContext context) => _context = context;

        public async Task<CredLineaCredito> ResolverLineaCreditoAsync(int nSubProd, decimal montoSolicitado)
        {
            var linea = await _context.CredLineaCreditos
                .Where(l => l.NSubProd == nSubProd && l.BEstado
                    && montoSolicitado >= l.NMontoMin && montoSolicitado <= l.NMontoMax)
                .FirstOrDefaultAsync();

            if (linea is null)
                throw new BusinessRuleException(
                    CreditErrors.CreditLineNotConfigured(nSubProd, montoSolicitado));

            return linea;
        }
    }
}
