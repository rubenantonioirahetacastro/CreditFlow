using CreditFlow.API.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Credit.Web.GetCreditEvaluation
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EvaluacionCreditoDetalleController : ControllerBase
    {
        private const int CodigoCatalogoEstadosCredito = 116;
        private const int CodigoCatalogoSubProducto = 109;
        private const int CodigoCatalogoTipoGarantia = 103;
        private const int CodigoCatalogoTipoNegocio = 106;
        private const int CodigoCatalogoDocumentoResidencia = 111;
        private const int CodigoCatalogoMarcaGarantia = 121;
        private const int CodigoCatalogoAnioGarantia = 122;

        private readonly DbNegocioContext _context;

        public EvaluacionCreditoDetalleController(DbNegocioContext context)
        {
            _context = context;
        }

        // GET api/EvaluacionCreditoDetalle/{nCodAge}/{nCodCred}
        [HttpGet("{nCodAge}/{nCodCred}")]
        public async Task<IActionResult> Obtener(int nCodAge, int nCodCred)
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

            var agencia = await _context.Agencias.AsNoTracking()
                .Where(a => a.NCodAge == credito.NCodAge)
                .Select(a => a.CNomAge)
                .FirstOrDefaultAsync();

            var estadoNombre = await _context.CatalogoCodigos.AsNoTracking()
                .Where(x => x.NCodigo == CodigoCatalogoEstadosCredito && x.NValor == credito.NEstado)
                .Select(x => x.CNomCod)
                .FirstOrDefaultAsync();

            var subProductoNombre = await _context.CatalogoCodigos.AsNoTracking()
                .Where(x => x.NCodigo == CodigoCatalogoSubProducto && x.NValor == credito.NSubProd)
                .Select(x => x.CNomCod)
                .FirstOrDefaultAsync();

            var codigosDescriptivos = new[]
            {
                CodigoCatalogoTipoGarantia,
                CodigoCatalogoTipoNegocio,
                CodigoCatalogoDocumentoResidencia,
                CodigoCatalogoMarcaGarantia,
                CodigoCatalogoAnioGarantia,
            };
            var catalogosDescriptivos = await _context.CatalogoCodigos.AsNoTracking()
                .Where(x => codigosDescriptivos.Contains(x.NCodigo))
                .Select(x => new { x.NCodigo, x.NValor, x.CNomCod })
                .ToListAsync();

            string? NombreCatalogo(int codigo, int valor) => catalogosDescriptivos
                .FirstOrDefault(x => x.NCodigo == codigo && x.NValor == valor)
                ?.CNomCod;

            var negocio = credito.IdNegocio.HasValue
                ? await _context.Negocios.AsNoTracking().FirstOrDefaultAsync(n => n.IdNegocio == credito.IdNegocio.Value)
                : null;

            var fotosNegocio = negocio != null
                ? await _context.FotoNegocios.AsNoTracking().Where(f => f.IdNegocio == negocio.IdNegocio).ToListAsync()
                : new List<Features.Credit.Shared.Domain.Models.FotoNegocio>();

            var fotosDocumentoIdentidad = await _context.FotoIds.AsNoTracking()
                .Where(f => f.IdPersona == persona.IdPersona)
                .OrderBy(f => f.NTipoFoto)
                .ThenBy(f => f.IdFoto)
                .ToListAsync();

            var capacidadPago = credito.IdCapacidadPago.HasValue
                ? await _context.CapacidadPagos.AsNoTracking().FirstOrDefaultAsync(c => c.IdCapacidadPago == credito.IdCapacidadPago.Value)
                : null;

            var fotosDocumentacion = credito.IdDocumentacion.HasValue
                ? await _context.FotoDocumentacions.AsNoTracking().Where(f => f.IdDocumentacion == credito.IdDocumentacion.Value).ToListAsync()
                : new List<Features.Credit.Shared.Domain.Models.FotoDocumentacion>();

            var fiador = credito.IdFiador.HasValue
                ? await _context.Fiadors.AsNoTracking().FirstOrDefaultAsync(f => f.IdFiador == credito.IdFiador.Value)
                : null;

            var fotosGarantia = credito.IdGarantia.HasValue
                ? await _context.GarantiaFotos.AsNoTracking().Where(f => f.IdGarantia == credito.IdGarantia.Value).ToListAsync()
                : new List<Features.Credit.Shared.Domain.Models.GarantiaFoto>();

            var garantia = credito.IdGarantia.HasValue
                ? await _context.Garantia.AsNoTracking().FirstOrDefaultAsync(g => g.IdGarantia == credito.IdGarantia.Value)
                : null;

            var tasaLineaCredito = credito.NCodLinea.HasValue
                ? await _context.CredLineaCreditos.AsNoTracking()
                    .Where(l => l.NCodLinea == credito.NCodLinea.Value)
                    .Select(l => (decimal?)l.NTasaCom)
                    .FirstOrDefaultAsync()
                : null;

            var periodicidad = await _context.LineaCatalogoAuxiliars.AsNoTracking()
                .Where(l => l.NProd == credito.NProd && l.NSubProd == credito.NSubProd)
                .Select(l => l.NPeriodicidad)
                .FirstOrDefaultAsync();

            var plazoMeses = periodicidad is > 0
                ? credito.NNroCuotas / periodicidad.Value
                : credito.NPeriodo;

            var verificaciones = await _context.VerificacionCreditos.AsNoTracking()
                .Where(v => v.NCodAge == nCodAge && v.NCodCred == nCodCred)
                .OrderByDescending(v => v.DFecha)
                .ToListAsync();

            var hoy = DateOnly.FromDateTime(DateTime.UtcNow);
            var edad = persona.DFechaNacimiento is DateOnly nacimiento
                ? hoy.Year - nacimiento.Year - (hoy < nacimiento.AddYears(hoy.Year - nacimiento.Year) ? 1 : 0)
                : (int?)null;

            var valorGarantiaTotal = garantia?.NValor ?? 0m;

            var dto = new EvaluacionCreditoDetalleDto
            {
                Cliente = new ClienteEvaluacionDto
                {
                    IdPersona = persona.IdPersona,
                    NombreCompleto = $"{persona.CNombres} {persona.CPrimerApellido} {persona.CSegundoApellido}".Trim(),
                    CDocumento = persona.CDocumento,
                    DFechaVencimientoDocumento = persona.DFechaVencimiento,
                    Edad = edad,
                    NEstadoCivil = persona.NEstadoCivil,
                    NProfesion = persona.NProfesion,
                    CCorreo = persona.CCorreo,
                    CCelular = persona.CCelular,
                    FotoUrl = persona.VFotoPerfil,
                    FotosDocumento = fotosDocumentoIdentidad
                        .Select(f => new FotoEvaluacionDto { IdFoto = f.IdFoto, TipoFoto = f.NTipoFoto })
                        .ToList(),
                },
                Credito = new CreditoEvaluacionDto
                {
                    NCodAge = credito.NCodAge,
                    NCodCred = credito.NCodCred,
                    NCodLinea = credito.NCodLinea,
                    Agencia = agencia,
                    NProd = credito.NProd,
                    NSubProd = credito.NSubProd,
                    SubProducto = subProductoNombre,
                    NPrestamo = credito.NPrestamo,
                    NPeriodo = credito.NPeriodo,
                    PlazoMeses = plazoMeses,
                    NNroCuotas = credito.NNroCuotas,
                    NTasaComp = credito.NTasaComp ?? tasaLineaCredito,
                    NEstado = credito.NEstado,
                    EstadoNombre = estadoNombre,
                    DFecVig = credito.DFecVig,
                    UsuarioGestion = persona.CUsuarioGestion,
                },
                Negocio = negocio == null ? null : new NegocioEvaluacionDto
                {
                    CNombre = negocio.CNombre,
                    CDireccion = negocio.CDireccion,
                    CTelefono = negocio.CTelefono,
                    CSector = negocio.CSector,
                    TipoNegocio = NombreCatalogo(CodigoCatalogoTipoNegocio, negocio.CSector),
                    THoraInicio = negocio.THoraInicio?.ToString("HH:mm"),
                    THoraCierre = negocio.THoraCierre?.ToString("HH:mm"),
                    CGeolocalizacion = negocio.CGeolocalizacion,
                    Fotos = fotosNegocio.Select(f => new FotoEvaluacionDto { IdFoto = f.IdFoto, TipoFoto = f.NTipoFoto }).ToList(),
                },
                CapacidadPago = capacidadPago == null ? null : new CapacidadPagoEvaluacionDto
                {
                    DGastosEducacion = capacidadPago.DGastosEducacion,
                    DGastosAlimentacion = capacidadPago.DGastosAlimentacion,
                    DGastosSalud = capacidadPago.DGastosSalud,
                    DOtrosGastos = capacidadPago.DOtrosGastos,
                    DOtrosIngresos = capacidadPago.DOtrosIngresos,
                },
                Documentacion = new DocumentacionEvaluacionDto
                {
                    Fotos = fotosDocumentacion.Select(f => new FotoEvaluacionDto
                    {
                        IdFoto = f.IdFoto,
                        TipoFoto = f.IdTipoDocumentacion,
                        TipoFotoNombre = NombreCatalogo(CodigoCatalogoDocumentoResidencia, f.IdTipoDocumentacion),
                    }).ToList(),
                },
                Fiador = fiador == null ? null : new FiadorEvaluacionDto
                {
                    NombreCompleto = $"{fiador.CNombres} {fiador.CPrimerApellido} {fiador.CSegundoApellido}".Trim(),
                    CDocumento = fiador.CDocumento,
                    CDireccion = fiador.CDireccion,
                    CCelular = fiador.CCelular,
                },
                Garantia = new GarantiaEvaluacionDto
                {
                    NTipoGarantia = garantia?.NTipoGarantia ?? 0,
                    TipoGarantia = garantia == null
                        ? null
                        : NombreCatalogo(CodigoCatalogoTipoGarantia, garantia.NTipoGarantia),
                    NMarca = garantia?.NMarca ?? 0,
                    Marca = garantia == null
                        ? null
                        : NombreCatalogo(CodigoCatalogoMarcaGarantia, garantia.NMarca),
                    NAnio = garantia?.NAnio ?? 0,
                    Anio = garantia == null
                        ? null
                        : NombreCatalogo(CodigoCatalogoAnioGarantia, garantia.NAnio),
                    Fotos = fotosGarantia.Select(f => new FotoGarantiaEvaluacionDto
                    {
                        IdFoto = f.IdFoto,
                    }).ToList(),
                    ValorTotal = valorGarantiaTotal,
                    CoberturaPorcentaje = credito.NPrestamo > 0 ? Math.Round(valorGarantiaTotal / credito.NPrestamo * 100, 1) : null,
                },
                Verificaciones = verificaciones.Select(v => new VerificacionEvaluacionDto
                {
                    CNombre = v.CNombre,
                    DFecha = v.DFecha,
                    NLatitud = v.NLatitud,
                    NLongitud = v.NLongitud,
                }).ToList(),
            };

            return Ok(dto);
        }
    }
}
