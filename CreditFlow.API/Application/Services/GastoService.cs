using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Infrastructure.Data;
using CreditFlow.API.Application.Interfaces;
using CreditFlow.API.Application.Requests;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Application.Services
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
