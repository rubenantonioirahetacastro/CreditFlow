using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Features.Credit.Shared.Contracts;
using CreditFlow.API.Features.Credit.Shared.Domain.Models;
using CreditFlow.API.Features.Credit.Shared.Validation;
using CreditFlow.API.Core.Storage;
using CreditFlow.API.Core.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;

namespace CreditFlow.API.Features.Credit.Mobile.CreateCredit
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
            GarantiaRequestValidator.ValidateCustomerData(request.Garantia);

                string cUsuarioGestion;
                if (User.Identity?.IsAuthenticated == true)
                {
                    var roleIdClaim = User.FindFirst(CustomClaimTypes.RoleId)?.Value;
                    var esCliente = int.TryParse(roleIdClaim, out var roleId) &&
                                    roleId == RoleIds.Client;

                    cUsuarioGestion = esCliente
                        ? "Autogestion"
                        : (User.Identity?.Name ?? "Empleado");
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
                        var path = await ImageUploadHelper.ValidateAndUploadAsync(f.Archivo, personaFolder, _blobService);

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
                        var path = await ImageUploadHelper.ValidateAndUploadAsync(f.Archivo, documentacionFolder, _blobService);

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
                        var path = await ImageUploadHelper.ValidateAndUploadAsync(f.Archivo, garantiaFolder, _blobService);

                        garantiaFotos.Add(new GarantiaFoto
                        {
                            IdFoto = f.IdFoto,
                            VFoto = path ?? f.VFoto ?? string.Empty
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
                        var path = await ImageUploadHelper.ValidateAndUploadAsync(f.Archivo, negocioFolder, _blobService);

                        fotoNegocios.Add(new FotoNegocio
                        {
                            IdFoto = f.IdFoto,
                            VFoto = path ?? f.VFoto,
                            NTipoFoto = f.NTipoFoto
                        });
                    }
                }

                // Foto de perfil: se guarda en UsuarioLogin.VFoto (cliente/{documento}), no en Persona
                var vFotoPerfil = await ImageUploadHelper.ValidateAndUploadAsync(
                    request.Persona.FotoPerfil, $"cliente/{request.Persona.CDocumento}", _blobService);

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
                    CDireccion = request.Persona.CDireccion,
                    CTelefono = request.Persona.CTelefono,
                    CCelular = request.Persona.CCelular,
                    CUsuarioGestion = cUsuarioGestion
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

                var garantia = new Garantium
                {
                    NTipoGarantia = request.Garantia?.NTipoGarantia ?? 0,
                    NMarca = request.Garantia?.NMarca ?? 0,
                    NAnio = request.Garantia?.NAnio ?? 0,
                    NValor = request.Garantia?.NValor ?? 0
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
                    garantia,
                    persona,
                    conyuge,
                    fiador,
                    negocio,
                    capacidadPago,
                    compra,
                    venta,
                    credito,
                    vFotoPerfil
                );

                return Ok(new CrearSolicitudCreditoResponse(filas));
        }
    }
}
