using CreditFlow.API.Core.Storage;
using CreditFlow.API.Features.Credit.Shared.Domain.Models;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Credit.Mobile.GetCreditDetail
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

        // GET api/BuscarSolicitudCredito/{nCodAge}/{nCodCred}/detalle
        [HttpGet("{nCodAge}/{nCodCred}/detalle")]
        public async Task<IActionResult> ObtenerDetalle(int nCodAge, int nCodCred)
        {
            var credito = await _context.Creditos.AsNoTracking()
                .FirstOrDefaultAsync(c => c.NCodAge == nCodAge && c.NCodCred == nCodCred);

            if (credito == null)
                return NotFound(new { Mensaje = "No se encontró la solicitud indicada." });

            var persona = credito.IdPersona.HasValue
                ? await _context.Personas.AsNoTracking().FirstOrDefaultAsync(p => p.IdPersona == credito.IdPersona.Value)
                : null;

            if (persona == null)
                return NotFound(new { Mensaje = "La solicitud no tiene un cliente asociado." });

            var conyuge = credito.IdConyuge.HasValue
                ? await _context.Conyuges.AsNoTracking().FirstOrDefaultAsync(x => x.IdConyuge == credito.IdConyuge.Value)
                : null;

            var fiador = credito.IdFiador.HasValue
                ? await _context.Fiadors.AsNoTracking().FirstOrDefaultAsync(x => x.IdFiador == credito.IdFiador.Value)
                : null;

            var negocio = credito.IdNegocio.HasValue
                ? await _context.Negocios.AsNoTracking().FirstOrDefaultAsync(x => x.IdNegocio == credito.IdNegocio.Value)
                : null;

            var capacidadPago = credito.IdCapacidadPago.HasValue
                ? await _context.CapacidadPagos.AsNoTracking().FirstOrDefaultAsync(x => x.IdCapacidadPago == credito.IdCapacidadPago.Value)
                : null;

            var garantia = credito.IdGarantia.HasValue
                ? await _context.Garantia.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.IdGarantia == credito.IdGarantia.Value)
                : null;

            var fotosId = await _context.FotoIds.AsNoTracking()
                .Where(f => f.IdPersona == persona.IdPersona)
                .ToListAsync();

            var fotosDocumentacion = credito.IdDocumentacion.HasValue
                ? await _context.FotoDocumentacions.AsNoTracking()
                    .Where(f => f.IdDocumentacion == credito.IdDocumentacion.Value).ToListAsync()
                : new List<FotoDocumentacion>();

            var fotosGarantia = credito.IdGarantia.HasValue
                ? await _context.GarantiaFotos.AsNoTracking()
                    .Where(f => f.IdGarantia == credito.IdGarantia.Value).ToListAsync()
                : new List<GarantiaFoto>();

            var ventas = new List<Venta>();
            var compras = new List<Compra>();
            var fotosNegocio = new List<FotoNegocio>();

            if (negocio != null)
            {
                ventas = await _context.Ventas.AsNoTracking().Where(v => v.IdNegocio == negocio.IdNegocio).ToListAsync();
                compras = await _context.Compras.AsNoTracking().Where(c => c.IdNegocio == negocio.IdNegocio).ToListAsync();
                fotosNegocio = await _context.FotoNegocios.AsNoTracking().Where(f => f.IdNegocio == negocio.IdNegocio).ToListAsync();
            }

            var estadoNombre = await _context.CatalogoCodigos.AsNoTracking()
                .Where(x => x.NCodigo == 116 && x.NValor == credito.NEstado)
                .Select(x => x.CNomCod)
                .FirstOrDefaultAsync();

            var dto = new SolicitudCreditoDetalleDto
            {
                Persona = new PersonaDetalleDto
                {
                    IdPersona = persona.IdPersona,
                    NTipoDocumento = persona.NTipoDocumento,
                    CDocumento = persona.CDocumento,
                    DFechaExpedicion = persona.DFechaExpedicion,
                    DFechaVencimiento = persona.DFechaVencimiento,
                    NDepartamentoDoc = persona.NDepartamentoDoc,
                    NMunicipioDoc = persona.NMunicipioDoc,
                    CNombres = persona.CNombres,
                    CPrimerApellido = persona.CPrimerApellido,
                    CSegundoApellido = persona.CSegundoApellido,
                    NSexo = persona.NSexo,
                    NNacionalidad = persona.NNacionalidad,
                    DFechaNacimiento = persona.DFechaNacimiento,
                    NDepartamentoNacimiento = persona.NDepartamentoNacimiento,
                    NMunicipioNacimiento = persona.NMunicipioNacimiento,
                    NEstadoCivil = persona.NEstadoCivil,
                    NProfesion = persona.NProfesion,
                    NEscolaridad = persona.NEscolaridad,
                    CCorreo = persona.CCorreo,
                    CDireccion = persona.CDireccion,
                    CTelefono = persona.CTelefono,
                    CCelular = persona.CCelular,
                    VFotoPerfil = persona.VFotoPerfil,
                    Fotos = fotosId.Select(f => new FotoDto { IdFoto = f.IdFoto, Ruta = f.VFoto, TipoFoto = f.NTipoFoto }).ToList()
                },
                Conyuge = conyuge == null ? null : new ConyugeDetalleDto
                {
                    IdConyuge = conyuge.IdConyuge,
                    CNombres = conyuge.CNombres,
                    CPrimerApellido = conyuge.CPrimerApellido,
                    CSegundoApellido = conyuge.CSegundoApellido,
                    NTipoDocumento = conyuge.NTipoDocumento,
                    CDocumento = conyuge.CDocumento,
                    CTelefono = conyuge.CTelefono,
                    CCelular = conyuge.CCelular
                },
                Negocio = negocio == null ? null : new NegocioDetalleDto
                {
                    IdNegocio = negocio.IdNegocio,
                    CNombre = negocio.CNombre,
                    CDireccion = negocio.CDireccion,
                    CSector = negocio.CSector,
                    THoraInicio = negocio.THoraInicio?.ToString("HH:mm"),
                    THoraCierre = negocio.THoraCierre?.ToString("HH:mm"),
                    CTelefono = negocio.CTelefono,
                    CGeolocalizacion = negocio.CGeolocalizacion,
                    Ventas = ventas.Select(v => new VentaDetalleDto
                    {
                        CProducto = v.CProducto,
                        NCantidadVenta = v.NCantidadVenta,
                        NUnidadMedida = v.NUnidadMedida,
                        NPrecioXunidad = v.NPrecioXunidad,
                        NPrecioTotal = v.NPrecioTotal
                    }).ToList(),
                    Compras = compras.Select(c => new CompraDetalleDto
                    {
                        CProducto = c.CProducto,
                        NCantidadCompra = c.NCantidadCompra,
                        NUnidadMedida = c.NUnidadMedida,
                        NPrecioXunidad = c.NPrecioXunidad,
                        NPrecioTotal = c.NPrecioTotal
                    }).ToList(),
                    Fotos = fotosNegocio.Select(f => new FotoDto { IdFoto = f.IdFoto, Ruta = f.VFoto, TipoFoto = f.NTipoFoto }).ToList()
                },
                CapacidadPago = capacidadPago == null ? null : new CapacidadPagoDetalleDto
                {
                    IdCapacidadPago = capacidadPago.IdCapacidadPago,
                    DGastosEducacion = capacidadPago.DGastosEducacion,
                    DGastosAlimentacion = capacidadPago.DGastosAlimentacion,
                    DGastosSalud = capacidadPago.DGastosSalud,
                    DOtrosGastos = capacidadPago.DOtrosGastos,
                    DOtrosIngresos = capacidadPago.DOtrosIngresos
                },
                Documentacion = new DocumentacionDetalleDto
                {
                    IdDocumentacion = credito.IdDocumentacion ?? 0,
                    Fotos = fotosDocumentacion.Select(f => new FotoDto { IdFoto = f.IdFoto, Ruta = f.VFoto, TipoFoto = f.IdTipoDocumentacion }).ToList()
                },
                Fiador = fiador == null ? null : new FiadorDetalleDto
                {
                    IdFiador = fiador.IdFiador,
                    CNombres = fiador.CNombres,
                    CPrimerApellido = fiador.CPrimerApellido,
                    CSegundoApellido = fiador.CSegundoApellido,
                    NTipoDocumento = fiador.NTipoDocumento,
                    CDocumento = fiador.CDocumento,
                    CDireccion = fiador.CDireccion,
                    CTelefono = fiador.CTelefono,
                    CCelular = fiador.CCelular
                },
                Garantia = new GarantiaDetalleDto
                {
                    IdGarantia = credito.IdGarantia ?? 0,
                    NTipoGarantia = garantia?.NTipoGarantia ?? 0,
                    NMarca = garantia?.NMarca ?? 0,
                    NAnio = garantia?.NAnio ?? 0,
                    NValor = garantia?.NValor ?? 0,
                    Fotos = fotosGarantia.Select(f => new FotoGarantiaDto
                    {
                        IdFoto = f.IdFoto,
                        Ruta = f.VFoto
                    }).ToList()
                },
                Credito = new CreditoDetalleDto
                {
                    NCodAge = credito.NCodAge,
                    NCodCred = credito.NCodCred,
                    NProd = credito.NProd,
                    NSubProd = credito.NSubProd,
                    NPrestamo = credito.NPrestamo,
                    NSaldoK = credito.NSaldoK,
                    NCodLinea = credito.NCodLinea,
                    NEstado = credito.NEstado,
                    EstadoNombre = estadoNombre,
                    NPeriodo = credito.NPeriodo,
                    NNroCuotas = credito.NNroCuotas,
                    NCobroEnAgencia = credito.NCobroEnAgencia,
                    NAceptaTerminos = credito.NAceptaTerminos,
                    DFecVig = credito.DFecVig
                }
            };

            return Ok(dto);
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

        // GET api/BuscarSolicitudCredito/foto/persona/{idFoto}
        [HttpGet("foto/persona/{idFoto}")]
        public async Task<IActionResult> ObtenerFotoPersona(int idFoto)
        {
            var ruta = await _context.FotoIds.AsNoTracking()
                .Where(f => f.IdFoto == idFoto)
                .Select(f => f.VFoto)
                .FirstOrDefaultAsync();

            return await ServirFotoAsync(ruta);
        }

        // GET api/BuscarSolicitudCredito/foto/documentacion/{idFoto}
        [HttpGet("foto/documentacion/{idFoto}")]
        public async Task<IActionResult> ObtenerFotoDocumentacion(int idFoto)
        {
            var ruta = await _context.FotoDocumentacions.AsNoTracking()
                .Where(f => f.IdFoto == idFoto)
                .Select(f => f.VFoto)
                .FirstOrDefaultAsync();

            return await ServirFotoAsync(ruta);
        }

        // GET api/BuscarSolicitudCredito/foto/negocio/{idFoto}
        [HttpGet("foto/negocio/{idFoto}")]
        public async Task<IActionResult> ObtenerFotoNegocio(int idFoto)
        {
            var ruta = await _context.FotoNegocios.AsNoTracking()
                .Where(f => f.IdFoto == idFoto)
                .Select(f => f.VFoto)
                .FirstOrDefaultAsync();

            return await ServirFotoAsync(ruta);
        }

        // GET api/BuscarSolicitudCredito/foto/garantia/{idFoto}
        [HttpGet("foto/garantia/{idFoto}")]
        public async Task<IActionResult> ObtenerFotoGarantia(int idFoto)
        {
            var ruta = await _context.GarantiaFotos.AsNoTracking()
                .Where(f => f.IdFoto == idFoto)
                .Select(f => f.VFoto)
                .FirstOrDefaultAsync();

            return await ServirFotoAsync(ruta);
        }

        private async Task<IActionResult> ServirFotoAsync(string? fotoPath)
        {
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
