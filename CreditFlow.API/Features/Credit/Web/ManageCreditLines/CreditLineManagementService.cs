using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Core.Errors;
using CreditFlow.API.Features.Credit.Shared.Errors;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Credit.Web.ManageCreditLines
{
    public class CreditLineManagementService : ICreditLineManagementService
    {
        private readonly DbNegocioContext _context;

        public CreditLineManagementService(DbNegocioContext context)
        {
            _context = context;
        }

        public async Task<List<CreditLineDto>> GetAllAsync()
        {
            return await _context.CredLineaCreditos
                .AsNoTracking()
                .OrderBy(l => l.CDescripcion)
                .Select(l => MapToDto(l))
                .ToListAsync();
        }

        public async Task<CreditLineDto?> GetByIdAsync(int id)
        {
            var linea = await _context.CredLineaCreditos.AsNoTracking().FirstOrDefaultAsync(l => l.NCodLinea == id);
            return linea == null ? null : MapToDto(linea);
        }

        public async Task<CreditLineDto> CreateAsync(CreateCreditLineRequest request, string? user)
        {
            CreditLineRequestValidator.Validate(
                request.Descripcion,
                request.PlazoMinimo,
                request.PlazoMaximo,
                request.MontoMinimo,
                request.MontoMaximo);

            var linea = new CredLineaCredito
            {
                CDescripcion = request.Descripcion.Trim(),
                NTasaCom = request.TasaComision,
                NProd = request.Producto,
                NSubProd = request.SubProducto,
                NPlazoMin = request.PlazoMinimo,
                NPlazoMax = request.PlazoMaximo,
                NMontoMin = request.MontoMinimo,
                NMontoMax = request.MontoMaximo,
                NNumPresMin = request.NumeroPrestamosMinimo,
                NNumPresMax = request.NumeroPrestamosMaximo,
                BRefinan = request.AplicaRefinanciamiento,
                CUser = user,
                BEstado = true
            };

            await _context.CredLineaCreditos.AddAsync(linea);
            await _context.SaveChangesAsync();

            return MapToDto(linea);
        }

        public async Task<CreditLineDto?> UpdateAsync(int id, UpdateCreditLineRequest request, string? user)
        {
            var linea = await _context.CredLineaCreditos.FirstOrDefaultAsync(l => l.NCodLinea == id);
            if (linea == null)
                return null;

            CreditLineRequestValidator.Validate(
                request.Descripcion,
                request.PlazoMinimo,
                request.PlazoMaximo,
                request.MontoMinimo,
                request.MontoMaximo);

            linea.CDescripcion = request.Descripcion.Trim();
            linea.NTasaCom = request.TasaComision;
            linea.NProd = request.Producto;
            linea.NSubProd = request.SubProducto;
            linea.NPlazoMin = request.PlazoMinimo;
            linea.NPlazoMax = request.PlazoMaximo;
            linea.NMontoMin = request.MontoMinimo;
            linea.NMontoMax = request.MontoMaximo;
            linea.NNumPresMin = request.NumeroPrestamosMinimo;
            linea.NNumPresMax = request.NumeroPrestamosMaximo;
            linea.BRefinan = request.AplicaRefinanciamiento;
            linea.BEstado = request.Activa;
            linea.CUser = user ?? linea.CUser;

            await _context.SaveChangesAsync();

            return MapToDto(linea);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var linea = await _context.CredLineaCreditos.FirstOrDefaultAsync(l => l.NCodLinea == id);
            if (linea == null)
                return false;

            _context.CredLineaCreditos.Remove(linea);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // La línea está referenciada por créditos u otros catálogos (FK). En vez de
                // borrarla físicamente, se recomienda desactivarla vía PUT (Activa = false).
                throw new ResourceConflictException(CreditErrors.CreditLineInUse);
            }

            return true;
        }

        private static CreditLineDto MapToDto(CredLineaCredito l) => new()
        {
            NCodLinea = l.NCodLinea,
            Descripcion = l.CDescripcion,
            TasaComision = l.NTasaCom,
            Producto = l.NProd,
            SubProducto = l.NSubProd,
            PlazoMinimo = l.NPlazoMin,
            PlazoMaximo = l.NPlazoMax,
            MontoMinimo = l.NMontoMin,
            MontoMaximo = l.NMontoMax,
            NumeroPrestamosMinimo = l.NNumPresMin,
            NumeroPrestamosMaximo = l.NNumPresMax,
            AplicaRefinanciamiento = l.BRefinan,
            Usuario = l.CUser,
            Activa = l.BEstado
        };
    }
}
