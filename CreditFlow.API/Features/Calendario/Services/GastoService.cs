using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Infrastructure.Data;
using CreditFlow.API.Features.Calendario.Services;
using CreditFlow.API.Features.Calendario.Requests;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Calendario.Services
{
    public class GastoService : IGastoService
    {
        private readonly DbNegocioContext _context;

        public GastoService(DbNegocioContext context)
        {
            _context = context;
        }

        public async Task<decimal> ObtenerGastoAsync(CreditoRequest request)
        {
            // Implementa tu lógica de cálculo de gastos aquí
            // Este es un ejemplo básico, ajusta según tu lógica de negocio
            
            var gasto = await _context.CredGastos
                .Where(g => g.NProd == request.nProd 
                    && g.NSubProd == request.nSubProd
                    && g.NTipoGasto == request.nTipoGasto)
                .FirstOrDefaultAsync();

            return gasto?.NValor ?? 0m;
        }
    }
}
