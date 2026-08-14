using CreditFlow.API.Infrastructure.Data;
using CreditFlow.API.Features.BandejaVerificacion.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.BandejaVerificacion.Controllers
{
    [Route("api/Credito")]
    [ApiController]
    public class BandejaVerificacionController : ControllerBase
    {
        private readonly DbNegocioContext _context;

        public BandejaVerificacionController(DbNegocioContext context)
        {
            _context = context;
        }

        [HttpGet("listaCreditos-cpc")]
        public async Task<IActionResult> ListarCreditos([FromQuery] int? ncodage)
        {
            try
            {
                var estados = new[] { 1, 2, 3 };

                var query = _context.Creditos.AsQueryable()
                    .Where(c => estados.Contains(c.NEstado));

                if (ncodage.HasValue)
                {
                    query = query.Where(c => c.NCodAge == ncodage.Value);
                }

                var lista = await (from c in query
                                   join p in _context.Personas on c.IdPersona equals p.IdPersona into ps
                                   from p in ps.DefaultIfEmpty()
                                   join a in _context.Agencias on c.NCodAge equals a.NCodAge into ags
                                   from a in ags.DefaultIfEmpty()
                                   join cat in _context.CatalogoCodigos on new { codigo = 116, valor = c.NEstado } equals new { codigo = cat.NCodigo, valor = cat.NValor } into cats
                                   from cat in cats.DefaultIfEmpty()
                                   join catsub in _context.CatalogoCodigos on new { codigo = 109, valor = (int?)c.NSubProd } equals new { codigo = catsub.NCodigo, valor = (int?)catsub.NValor } into catsubs
                                   from catsub in catsubs.DefaultIfEmpty()
                                   select new CreditoListadoResponse
                                   {
                                       NCodCred = c.NCodCred,
                                       NCodAge = c.NCodAge,
                                       Agencia = a == null ? null : a.CNomAge,
                                       MontoSolicitado = c.NPrestamo,
                                       Estado = cat == null ? null : cat.CNomCod,
                                       NSubProd = c.NSubProd,
                                       SubProducto = catsub == null ? null : catsub.CNomCod,
                                       NombreCliente = p == null ? null : (p.CNombres + " " + p.CPrimerApellido + " " + (p.CSegundoApellido ?? "")).Trim()
                                   }).ToListAsync();

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensaje = $"Error interno del servidor: {ex.Message}" });
            }
        }
    }
}
