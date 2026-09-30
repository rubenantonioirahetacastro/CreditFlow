using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Credit.Shared.SubProductConfig
{
    [Route("api/LineaAuxiliar")]
    [ApiController]
    public class CreditLineOptionsController : ControllerBase
    {
        private const int ProductoCredito = 4;

        private readonly DbNegocioContext _context;

        public CreditLineOptionsController(DbNegocioContext context)
        {
            _context = context;
        }

        // GET api/LineaAuxiliar/1
        [HttpGet("{nSubProd}")]
        public async Task<IActionResult> GetCreditLineOptions(int nSubProd)
        {
            if (nSubProd <= 0)
                return BadRequest("El código de subproducto (nSubProd) debe ser mayor a 0.");

            var opciones = await (
                from lca in _context.LineaCatalogoAuxiliars.AsNoTracking()
                join cl in _context.CredLineaCreditos.AsNoTracking()
                    on lca.NSubProd equals cl.NSubProd
                where cl.NProd == ProductoCredito
                    && lca.NSubProd == nSubProd
                    && lca.NCatalogoCodigo.HasValue
                    && cl.BEstado
                orderby cl.NPlazoMin, cl.NPlazoMax
                select new
                {
                    MontoMin = cl.NMontoMin,
                    MontoMax = cl.NMontoMax,
                    PlazoMin = cl.NPlazoMin,
                    PlazoMax = cl.NPlazoMax,
                    NumeroCatalogo = lca.NCatalogoCodigo,
                    LineaCredito = cl.NCodLinea,
                    Periodicidad = lca.NPeriodicidad
                })
                .ToListAsync();

            if (opciones == null || opciones.Count == 0)
                return NotFound("No se encontraron opciones de línea de crédito para el subproducto proporcionado.");

            return Ok(opciones);
        }
    }
}
