using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Features.SolicitudCredito.Data.Dto;
using CreditFlow.API.Features.SolicitudCredito.Domain.Model;
using CreditFlow.API.Application.Interfaces;
using CreditFlow.API.Shared.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using System.Security.Claims;

namespace CreditFlow.API.Features.SolicitudCredito
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolicitudCreditoController : ControllerBase
    {
        private readonly ICrearSolicitudCreditoHandler _handler;
        private readonly IBlobStorageService _blobService;

        public SolicitudCreditoController(ICrearSolicitudCreditoHandler handler, IBlobStorageService blobService)
        {
            _handler = handler;
            _blobService = blobService;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Crear([FromForm] CrearSolicitudCreditoRequest request)
        {
            try
            {
                string cUsuarioGestion;
                if (User.Identity?.IsAuthenticated == true)
                {
                    var esCliente = User.FindAll(ClaimTypes.Role)
                        .Any(r => string.Equals(r.Value, "Usuario", StringComparison.OrdinalIgnoreCase));

                    cUsuarioGestion = esCliente
                        ? "Autogestion"
                        : (User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Empleado");
                }
                else
                {
                    cUsuarioGestion = "Nuevo";
                }

                var requestFolder = $"solicitudes/{Guid.NewGuid()}";
                var personaFolder = $"{requestFolder}/personas";
                var garantiaFolder = $"{requestFolder}/garantias";
                var negocioFolder = $"{requestFolder}/negocios";
                var documentacionFolder = $"{requestFolder}/documentacion";

                // Map FotoIds
                List<FotoId>? fotoIds = null;
                if (request.FotoIds != null)
                {
                    fotoIds = new List<FotoId>();
                    foreach (var f in request.FotoIds)
                    {
                        var path = await ImagenUploadHelper.ValidarYSubirAsync(f.Archivo, personaFolder, _blobService);

                        fotoIds.Add(new FotoId
                        {
                            IdFoto = f.IdFoto,
                            VFoto = path ?? f.VFoto ?? string.Empty,
                            NTipoFoto = f.NTipoFoto
                        });
                    }
                }

                // Map FotoDocumentacions
                List<FotoDocumentacion>? fotoDocs = null;
                if (request.FotoDocumentacions != null)
                {
                    fotoDocs = new List<FotoDocumentacion>();
                    foreach (var f in request.FotoDocumentacions)
                    {
                        var path = await ImagenUploadHelper.ValidarYSubirAsync(f.Archivo, documentacionFolder, _blobService);

                        fotoDocs.Add(new FotoDocumentacion
                        {
                            IdFoto = f.IdFoto,
                            VFoto = path ?? f.VFoto,
                            IdTipoDocumentacion = f.IdTipoDocumentacion
                        });
                    }
                }

                // Map GarantiaFotos
                List<GarantiaFoto>? garantiaFotos = null;
                if (request.GarantiaFotos != null)
                {
                    garantiaFotos = new List<GarantiaFoto>();
                    foreach (var f in request.GarantiaFotos)
                    {
                        var path = await ImagenUploadHelper.ValidarYSubirAsync(f.Archivo, garantiaFolder, _blobService);

                        garantiaFotos.Add(new GarantiaFoto
                        {
                            IdFoto = f.IdFoto,
                            VFoto = path ?? f.VFoto ?? string.Empty,
                            NValor = f.NValor,
                            IdArticuloGarantia = f.IdArticuloGarantia
                        });
                    }
                }

                // Map FotoNegocios
                List<FotoNegocio>? fotoNegocios = null;
                if (request.FotoNegocios != null)
                {
                    fotoNegocios = new List<FotoNegocio>();
                    foreach (var f in request.FotoNegocios)
                    {
                        var path = await ImagenUploadHelper.ValidarYSubirAsync(f.Archivo, negocioFolder, _blobService);

                        fotoNegocios.Add(new FotoNegocio
                        {
                            IdFoto = f.IdFoto,
                            VFoto = path ?? f.VFoto,
                            NTipoFoto = f.NTipoFoto
                        });
                    }
                }

                // Foto de perfil
                var vFotoPerfil = await ImagenUploadHelper.ValidarYSubirAsync(request.Persona.FotoPerfil, personaFolder, _blobService);

                // Map DTOs -> entidades de dominio (los campos que el servidor genera se
                var persona = new Persona
                {
                    NTipoDocumento = request.Persona.NTipoDocumento,
                    CDocumento = request.Persona.CDocumento,
                    DFechaExpedicion = request.Persona.DFechaExpedicion,
                    DFechaVencimiento = request.Persona.DFechaVencimiento,
                    NDepartamentoDoc = request.Persona.NDepartamentoDoc,
                    NMunicipioDoc = request.Persona.NMunicipioDoc,
                    CNombres = request.Persona.CNombres,
                    CPrimerApellido = request.Persona.CPrimerApellido,
                    CSegundoApellido = request.Persona.CSegundoApellido,
                    NSexo = request.Persona.NSexo,
                    NNacionalidad = request.Persona.NNacionalidad,
                    DFechaNacimiento = request.Persona.DFechaNacimiento,
                    NDepartamentoNacimiento = request.Persona.NDepartamentoNacimiento,
                    NMunicipioNacimiento = request.Persona.NMunicipioNacimiento,
                    NEstadoCivil = request.Persona.NEstadoCivil,
                    NProfesion = request.Persona.NProfesion,
                    NEscolaridad = request.Persona.NEscolaridad,
                    CCorreo = request.Persona.CCorreo,
                    CTelefono = request.Persona.CTelefono,
                    CCelular = request.Persona.CCelular,
                    CUsuarioGestion = cUsuarioGestion,
                    VFotoPerfil = vFotoPerfil
                };

                Conyuge? conyuge = request.Conyuge == null ? null : new Conyuge
                {
                    CNombres = request.Conyuge.CNombres,
                    CPrimerApellido = request.Conyuge.CPrimerApellido,
                    CSegundoApellido = request.Conyuge.CSegundoApellido,
                    NTipoDocumento = request.Conyuge.NTipoDocumento,
                    CDocumento = request.Conyuge.CDocumento,
                    CTelefono = request.Conyuge.CTelefono,
                    CCelular = request.Conyuge.CCelular
                };

                Fiador? fiador = request.Fiador == null ? null : new Fiador
                {
                    CNombres = request.Fiador.CNombres,
                    CPrimerApellido = request.Fiador.CPrimerApellido,
                    CSegundoApellido = request.Fiador.CSegundoApellido,
                    NTipoDocumento = request.Fiador.NTipoDocumento,
                    CDocumento = request.Fiador.CDocumento,
                    CDireccion = request.Fiador.CDireccion,
                    CTelefono = request.Fiador.CTelefono,
                    CCelular = request.Fiador.CCelular
                };

                Negocio? negocio = request.Negocio == null ? null : new Negocio
                {
                    CNombre = request.Negocio.CNombre,
                    CDireccion = request.Negocio.CDireccion,
                    CSector = request.Negocio.CSector,
                    THoraInicio = request.Negocio.THoraInicio,
                    THoraCierre = request.Negocio.THoraCierre,
                    CTelefono = request.Negocio.CTelefono,
                    CGeolocalizacion = request.Negocio.CGeolocalizacion
                };

                CapacidadPago? capacidadPago = request.CapacidadPago == null ? null : new CapacidadPago
                {
                    DGastosEducacion = request.CapacidadPago.DGastosEducacion,
                    DGastosAlimentacion = request.CapacidadPago.DGastosAlimentacion,
                    DGastosSalud = request.CapacidadPago.DGastosSalud,
                    DOtrosGastos = request.CapacidadPago.DOtrosGastos,
                    DOtrosIngresos = request.CapacidadPago.DOtrosIngresos
                };

                var credito = new Credito
                {
                    NProd = request.Credito.NProd,
                    NSubProd = request.Credito.NSubProd,
                    NPrestamo = request.Credito.NPrestamo,
                    NPeriodo = request.Credito.NPeriodo,
                    NNroCuotas = request.Credito.NNroCuotas,
                    NMontoCuota = request.Credito.NMontoCuota,
                    NCobroEnAgencia = request.Credito.NCobroEnAgencia,
                    NAceptaTerminos = request.Credito.NAceptaTerminos
                };

                List<Compra>? compra = request.Compra?
                    .Select(c => new Compra
                    {
                        CProducto = c.CProducto,
                        NCantidadCompra = c.NCantidadCompra,
                        NUnidadMedida = c.NUnidadMedida,
                        NPrecioXunidad = c.NPrecioXunidad,
                        NPrecioTotal = c.NPrecioTotal
                    })
                    .ToList();

                List<Venta>? venta = request.Venta?
                    .Select(v => new Venta
                    {
                        CProducto = v.CProducto,
                        NCantidadVenta = v.NCantidadVenta,
                        NUnidadMedida = v.NUnidadMedida,
                        NPrecioXunidad = v.NPrecioXunidad,
                        NPrecioTotal = v.NPrecioTotal
                    })
                    .ToList();

                var filas = await _handler.EjecutarAsync(
                    fotoIds,
                    fotoDocs,
                    garantiaFotos,
                    fotoNegocios,
                    persona,
                    conyuge,
                    fiador,
                    negocio,
                    capacidadPago,
                    compra,
                    venta,
                    credito
                );

                return Ok(new CrearSolicitudCreditoResponse(filas));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
