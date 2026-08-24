using CreditFlow.API.Application.Interfaces;
using CreditFlow.API.Features.SolicitudCredito.Buscar.Dto;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.SolicitudCredito
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BuscarSolicitudCreditoController : ControllerBase
    {
        private readonly DbNegocioContext _context;
        private readonly IBlobStorageService _blobService;

        public BuscarSolicitudCreditoController(DbNegocioContext context, IBlobStorageService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        // GET api/BuscarSolicitudCredito
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var solicitudes = await (
                from c in _context.Creditos.AsNoTracking()
                join p in _context.Personas.AsNoTracking() on c.IdPersona equals p.IdPersona
                where c.NEstado == 1 || c.NEstado == 2
                orderby c.DFecVig descending
                select new SolicitudCreditoResumenDto
                {
                    IdPersona = p.IdPersona,
                    NCodAge = c.NCodAge,
                    NCodCred = c.NCodCred,
                    CDocumento = p.CDocumento,
                    CNombres = p.CNombres,
                    CPrimerApellido = p.CPrimerApellido,
                    CSegundoApellido = p.CSegundoApellido,
                    FotoUrl = p.VFotoPerfil,
                    CUsuarioGestion = p.CUsuarioGestion,
                    NProd = c.NProd,
                    NSubProd = c.NSubProd,
                    NPrestamo = c.NPrestamo,
                    NEstado = c.NEstado,
                    DFecVig = c.DFecVig
                }
            ).ToListAsync();

            return Ok(solicitudes);
        }

        // GET api/BuscarSolicitudCredito/{idPersona}/foto
        [HttpGet("{idPersona}/foto")]
        public async Task<IActionResult> ObtenerFoto(int idPersona)
        {
            var fotoPath = await _context.Personas.AsNoTracking()
                .Where(p => p.IdPersona == idPersona)
                .Select(p => p.VFotoPerfil)
                .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(fotoPath))
                return NotFound();

            var stream = await _blobService.DownloadImageAsync(fotoPath);
            if (stream == null)
                return NotFound();

            return File(stream, ObtenerContentType(Path.GetExtension(fotoPath)));
        }

        private static string ObtenerContentType(string extension) => extension.ToLowerInvariant() switch
        {
            ".png" => "image/png",
            ".webp" => "image/webp",
            _ => "image/jpeg"
        };
    }
}
