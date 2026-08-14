using CreditFlow.API.Application.DTOs.Mantenimientos;
using CreditFlow.API.Application.Interfaces.Mantenimientos;
using CreditFlow.API.Application.Requests.Mantenimientos;
using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Application.Services.Mantenimientos
{
    public class LineaCreditoAdminService : ILineaCreditoAdminService
    {
        private readonly DbNegocioContext _context;

        public LineaCreditoAdminService(DbNegocioContext context)
        {
            _context = context;
        }

        public async Task<List<LineaCreditoDto>> ObtenerTodasAsync()
        {
            return await _context.CredLineaCreditos
                .AsNoTracking()
                .OrderBy(l => l.CDescripcion)
                .Select(l => MapearADto(l))
                .ToListAsync();
        }

        public async Task<LineaCreditoDto?> ObtenerPorIdAsync(int id)
        {
            var linea = await _context.CredLineaCreditos.AsNoTracking().FirstOrDefaultAsync(l => l.NCodLinea == id);
            return linea == null ? null : MapearADto(linea);
        }

        public async Task<LineaCreditoDto> CrearAsync(CrearLineaCreditoRequest request, string? usuario)
        {
            ValidarRequest(request.Descripcion, request.PlazoMinimo, request.PlazoMaximo, request.MontoMinimo, request.MontoMaximo);

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
                CUser = usuario,
                BEstado = true
            };

            await _context.CredLineaCreditos.AddAsync(linea);
            await _context.SaveChangesAsync();

            return MapearADto(linea);
        }

        public async Task<LineaCreditoDto?> ActualizarAsync(int id, ActualizarLineaCreditoRequest request, string? usuario)
        {
            var linea = await _context.CredLineaCreditos.FirstOrDefaultAsync(l => l.NCodLinea == id);
            if (linea == null)
                return null;

            ValidarRequest(request.Descripcion, request.PlazoMinimo, request.PlazoMaximo, request.MontoMinimo, request.MontoMaximo);

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
            linea.CUser = usuario ?? linea.CUser;

            await _context.SaveChangesAsync();

            return MapearADto(linea);
        }

        public async Task<bool> EliminarAsync(int id)
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
                throw new InvalidOperationException(
                    "No se puede eliminar la línea de crédito porque está en uso. Desactívela en su lugar.");
            }

            return true;
        }

        private static void ValidarRequest(string descripcion, int plazoMinimo, int plazoMaximo, decimal montoMinimo, decimal montoMaximo)
        {
            if (string.IsNullOrWhiteSpace(descripcion))
                throw new ArgumentException("La descripción de la línea es requerida.");

            if (descripcion.Trim().Length > 150)
                throw new ArgumentException("La descripción no puede superar los 150 caracteres.");

            if (plazoMinimo > plazoMaximo)
                throw new ArgumentException("El plazo mínimo no puede ser mayor al plazo máximo.");

            if (montoMinimo > montoMaximo)
                throw new ArgumentException("El monto mínimo no puede ser mayor al monto máximo.");
        }

        private static LineaCreditoDto MapearADto(CredLineaCredito l) => new()
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
