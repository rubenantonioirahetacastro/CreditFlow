using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Features.Credit.Shared.Contracts;
using CreditFlow.API.Features.Credit.Shared.Domain.Models;
using CreditFlow.API.Features.Credit.Shared.Validation;
using CreditFlow.API.Infrastructure.Data;
using CreditFlow.API.Core.Storage;
using CreditFlow.API.Core.Security;
using CreditFlow.API.Core.Errors;
using CreditFlow.API.Features.Verification.Shared.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Verification.Mobile.SaveVerification;

[Route("api/SolicitudCredito")]
[ApiController]
[Authorize(Policy = AuthorizationPolicies.MobileVerifier)]
public class VerificarSolicitudCreditoController(
    DbNegocioContext context,
    IBlobStorageService blobService) : ControllerBase
{
    private const int EstadoEnAnalisis = 2;
    private const int EstadoVerificado = 5;

    [HttpPut("{nCodAge:int}/{nCodCred:int}/verificar")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Verificar(
        int nCodAge,
        int nCodCred,
        [FromQuery] VerificationLocationRequest location,
        [FromForm] CrearSolicitudCreditoRequest request)
    {
        GarantiaRequestValidator.ValidateCustomerData(request.Garantia);

        var credito = await context.Creditos
            .FirstOrDefaultAsync(c => c.NCodAge == nCodAge && c.NCodCred == nCodCred);

        if (credito == null)
            throw new ResourceNotFoundException(VerificationErrors.CreditNotFound);

        if (credito.NEstado != EstadoEnAnalisis)
        {
            throw new ResourceConflictException(VerificationErrors.CreditAlreadyProcessed);
        }

        if (await context.VerificacionCreditos.AnyAsync(x => x.NCodCred == nCodCred))
            throw new ResourceConflictException(VerificationErrors.AlreadyRegistered);

        if (!credito.IdPersona.HasValue)
            throw new ResourceConflictException(VerificationErrors.PersonNotAssigned);

        var persona = await context.Personas.FindAsync(credito.IdPersona.Value);
        if (persona == null)
            throw new ResourceConflictException(VerificationErrors.PersonNotFound);

        var verificador = await ObtenerVerificadorAsync();
        if (verificador == null)
            throw new UnauthorizedAppException(VerificationErrors.EmployeeNotFound);

        var latitude = location.NLatitud!.Value;
        var longitude = location.NLongitud!.Value;

        var requestFolder = $"verificaciones/{nCodAge}-{nCodCred}/{Guid.NewGuid()}";

        await using var transaction = await context.Database.BeginTransactionAsync();
            ActualizarPersona(persona, request.Persona);
            await ActualizarConyugeAsync(credito, request.Conyuge);
            await ActualizarFiadorAsync(credito, request.Fiador);
            await ActualizarNegocioAsync(
                credito,
                request.Negocio,
                request.Compra,
                request.Venta);
            await ActualizarCapacidadPagoAsync(credito, request.CapacidadPago);
            await ActualizarGarantiaAsync(credito, request.Garantia);

            await ActualizarFotosPersonaAsync(
                persona.IdPersona,
                request.FotoIds,
                $"{requestFolder}/personas");
            await ActualizarFotosNegocioAsync(
                credito.IdNegocio,
                request.FotoNegocios,
                $"{requestFolder}/negocios");
            await ActualizarFotosDocumentacionAsync(
                credito.IdDocumentacion,
                request.FotoDocumentacions,
                $"{requestFolder}/documentacion");
            await ActualizarFotosGarantiaAsync(
                credito.IdGarantia,
                request.GarantiaFotos,
                $"{requestFolder}/garantias");

            ActualizarCredito(credito, request.Credito);
            credito.NEstado = EstadoVerificado;
            context.VerificacionCreditos.Add(new VerificacionCredito
            {
                NCodCred = nCodCred,
                NCodAge = nCodAge,
                IdEmpleado = verificador.Value.IdEmpleado,
                CNombre = verificador.Value.Nombre,
                DFecha = DateTime.UtcNow,
                NLatitud = latitude,
                NLongitud = longitude
            });

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return NoContent();
    }

    private async Task<(int IdEmpleado, string Nombre)?> ObtenerVerificadorAsync()
    {
        var employeeClaim = User.FindFirst(CustomClaimTypes.EmployeeId)?.Value;
        if (!int.TryParse(employeeClaim, out var employeeId))
            return null;

        var empleado = await context.Empleados
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdEmpleado == employeeId);
        if (empleado == null)
            return null;

        var nombre = string.Join(
            " ",
            new[]
            {
                empleado.CNombres,
                empleado.CPrimerApellido,
                empleado.CSegundoApellido
            }.Where(value => !string.IsNullOrWhiteSpace(value)))
            .Trim();
        return (empleado.IdEmpleado, nombre);
    }

    private static void ActualizarPersona(Persona persona, PersonaRequest request)
    {
        persona.NTipoDocumento = request.NTipoDocumento;
        persona.CDocumento = request.CDocumento;
        persona.DFechaExpedicion = request.DFechaExpedicion;
        persona.DFechaVencimiento = request.DFechaVencimiento;
        persona.NDepartamentoDoc = request.NDepartamentoDoc;
        persona.NMunicipioDoc = request.NMunicipioDoc;
        persona.CNombres = request.CNombres;
        persona.CPrimerApellido = request.CPrimerApellido;
        persona.CSegundoApellido = request.CSegundoApellido;
        persona.NSexo = request.NSexo;
        persona.NNacionalidad = request.NNacionalidad;
        persona.DFechaNacimiento = request.DFechaNacimiento;
        persona.NDepartamentoNacimiento = request.NDepartamentoNacimiento;
        persona.NMunicipioNacimiento = request.NMunicipioNacimiento;
        persona.NEstadoCivil = request.NEstadoCivil;
        persona.NProfesion = request.NProfesion;
        persona.NEscolaridad = request.NEscolaridad;
        persona.CCorreo = request.CCorreo;
        persona.CDireccion = request.CDireccion;
        persona.CTelefono = request.CTelefono;
        persona.CCelular = request.CCelular;
        // La foto de perfil no se sube ni se modifica aquí: queda como se envió en la solicitud.
    }

    private async Task ActualizarConyugeAsync(Credito credito, ConyugeRequest? request)
    {
        var entity = credito.IdConyuge.HasValue
            ? await context.Conyuges.FindAsync(credito.IdConyuge.Value)
            : null;

        if (request == null)
        {
            if (entity != null)
                credito.IdConyuge = null;
            return;
        }

        if (entity == null)
        {
            entity = new Conyuge();
            context.Conyuges.Add(entity);
            await context.SaveChangesAsync();
            credito.IdConyuge = entity.IdConyuge;
        }

        entity.CNombres = request.CNombres;
        entity.CPrimerApellido = request.CPrimerApellido;
        entity.CSegundoApellido = request.CSegundoApellido;
        entity.NTipoDocumento = request.NTipoDocumento;
        entity.CDocumento = request.CDocumento;
        entity.CTelefono = request.CTelefono;
        entity.CCelular = request.CCelular;
    }

    private async Task ActualizarFiadorAsync(Credito credito, FiadorRequest? request)
    {
        var entity = credito.IdFiador.HasValue
            ? await context.Fiadors.FindAsync(credito.IdFiador.Value)
            : null;

        if (request == null)
        {
            if (entity != null)
                credito.IdFiador = null;
            return;
        }

        if (entity == null)
        {
            entity = new Fiador();
            context.Fiadors.Add(entity);
            await context.SaveChangesAsync();
            credito.IdFiador = entity.IdFiador;
        }

        entity.CNombres = request.CNombres;
        entity.CPrimerApellido = request.CPrimerApellido;
        entity.CSegundoApellido = request.CSegundoApellido;
        entity.NTipoDocumento = request.NTipoDocumento;
        entity.CDocumento = request.CDocumento;
        entity.CDireccion = request.CDireccion;
        entity.CTelefono = request.CTelefono;
        entity.CCelular = request.CCelular;
    }

    private async Task ActualizarNegocioAsync(
        Credito credito,
        NegocioRequest? request,
        List<CompraRequest>? compras,
        List<VentaRequest>? ventas)
    {
        var entity = credito.IdNegocio.HasValue
            ? await context.Negocios.FindAsync(credito.IdNegocio.Value)
            : null;

        if (request == null)
            throw new BusinessRuleException(VerificationErrors.BusinessRequired);

        if (entity == null)
        {
            entity = new Negocio();
            context.Negocios.Add(entity);
            await context.SaveChangesAsync();
            credito.IdNegocio = entity.IdNegocio;
        }

        entity.CNombre = request.CNombre;
        entity.CDireccion = request.CDireccion;
        entity.CSector = request.CSector;
        entity.THoraInicio = request.THoraInicio;
        entity.THoraCierre = request.THoraCierre;
        entity.CTelefono = request.CTelefono;
        entity.CGeolocalizacion = request.CGeolocalizacion;

        var comprasActuales = await context.Compras
            .Where(x => x.IdNegocio == entity.IdNegocio)
            .ToListAsync();
        var ventasActuales = await context.Ventas
            .Where(x => x.IdNegocio == entity.IdNegocio)
            .ToListAsync();
        context.Compras.RemoveRange(comprasActuales);
        context.Ventas.RemoveRange(ventasActuales);

        context.Compras.AddRange((compras ?? []).Select(item => new Compra
        {
            IdNegocio = entity.IdNegocio,
            CProducto = item.CProducto,
            NCantidadCompra = item.NCantidadCompra,
            NUnidadMedida = item.NUnidadMedida,
            NPrecioXunidad = item.NPrecioXunidad,
            NPrecioTotal = item.NPrecioTotal
        }));
        context.Ventas.AddRange((ventas ?? []).Select(item => new Venta
        {
            IdNegocio = entity.IdNegocio,
            CProducto = item.CProducto,
            NCantidadVenta = item.NCantidadVenta,
            NUnidadMedida = item.NUnidadMedida,
            NPrecioXunidad = item.NPrecioXunidad,
            NPrecioTotal = item.NPrecioTotal
        }));
    }

    private async Task ActualizarCapacidadPagoAsync(
        Credito credito,
        CapacidadPagoRequest? request)
    {
        var entity = credito.IdCapacidadPago.HasValue
            ? await context.CapacidadPagos.FindAsync(credito.IdCapacidadPago.Value)
            : null;

        if (request == null)
        {
            if (entity != null)
                credito.IdCapacidadPago = null;
            return;
        }

        if (entity == null)
        {
            entity = new CapacidadPago();
            context.CapacidadPagos.Add(entity);
            await context.SaveChangesAsync();
            credito.IdCapacidadPago = entity.IdCapacidadPago;
        }

        entity.DGastosEducacion = request.DGastosEducacion;
        entity.DGastosAlimentacion = request.DGastosAlimentacion;
        entity.DGastosSalud = request.DGastosSalud;
        entity.DOtrosGastos = request.DOtrosGastos;
        entity.DOtrosIngresos = request.DOtrosIngresos;
    }

    private async Task ActualizarFotosPersonaAsync(
        int idPersona,
        List<FotoIdRequest>? requests,
        string folder)
    {
        foreach (var request in requests ?? [])
        {
            var entity = request.IdFoto > 0
                ? await context.FotoIds.FirstOrDefaultAsync(x =>
                    x.IdFoto == request.IdFoto && x.IdPersona == idPersona)
                : null;
            var path = await ImageUploadHelper.ValidateAndUploadAsync(request.Archivo, folder, blobService);

            if (entity == null)
            {
                if (string.IsNullOrWhiteSpace(path)) continue;
                context.FotoIds.Add(new FotoId
                {
                    IdPersona = idPersona,
                    NTipoFoto = request.NTipoFoto,
                    VFoto = path
                });
            }
            else
            {
                entity.NTipoFoto = request.NTipoFoto;
                if (!string.IsNullOrWhiteSpace(path)) entity.VFoto = path;
            }
        }
    }

    private async Task ActualizarFotosNegocioAsync(
        int? idNegocio,
        List<FotoNegocioRequest>? requests,
        string folder)
    {
        if (!idNegocio.HasValue) return;
        foreach (var request in requests ?? [])
        {
            var entity = request.IdFoto > 0
                ? await context.FotoNegocios.FirstOrDefaultAsync(x =>
                    x.IdFoto == request.IdFoto && x.IdNegocio == idNegocio.Value)
                : null;
            var path = await ImageUploadHelper.ValidateAndUploadAsync(request.Archivo, folder, blobService);

            if (entity == null)
            {
                if (string.IsNullOrWhiteSpace(path)) continue;
                context.FotoNegocios.Add(new FotoNegocio
                {
                    IdNegocio = idNegocio.Value,
                    NTipoFoto = request.NTipoFoto,
                    VFoto = path
                });
            }
            else
            {
                entity.NTipoFoto = request.NTipoFoto;
                if (!string.IsNullOrWhiteSpace(path)) entity.VFoto = path;
            }
        }
    }

    private async Task ActualizarFotosDocumentacionAsync(
        int? idDocumentacion,
        List<FotoDocumentacionRequest>? requests,
        string folder)
    {
        if (!idDocumentacion.HasValue) return;
        foreach (var request in requests ?? [])
        {
            var entity = request.IdFoto > 0
                ? await context.FotoDocumentacions.FirstOrDefaultAsync(x =>
                    x.IdFoto == request.IdFoto && x.IdDocumentacion == idDocumentacion.Value)
                : null;
            var path = await ImageUploadHelper.ValidateAndUploadAsync(request.Archivo, folder, blobService);

            if (entity == null)
            {
                if (string.IsNullOrWhiteSpace(path)) continue;
                context.FotoDocumentacions.Add(new FotoDocumentacion
                {
                    IdDocumentacion = idDocumentacion.Value,
                    IdTipoDocumentacion = request.IdTipoDocumentacion,
                    VFoto = path
                });
            }
            else
            {
                entity.IdTipoDocumentacion = request.IdTipoDocumentacion;
                if (!string.IsNullOrWhiteSpace(path)) entity.VFoto = path;
            }
        }
    }

    private async Task ActualizarFotosGarantiaAsync(
        int? idGarantia,
        List<GarantiaFotoRequest>? requests,
        string folder)
    {
        if (!idGarantia.HasValue) return;
        foreach (var request in requests ?? [])
        {
            var entity = request.IdFoto > 0
                ? await context.GarantiaFotos.FirstOrDefaultAsync(x =>
                    x.IdFoto == request.IdFoto && x.IdGarantia == idGarantia.Value)
                : null;
            var path = await ImageUploadHelper.ValidateAndUploadAsync(request.Archivo, folder, blobService);

            if (entity == null)
            {
                if (string.IsNullOrWhiteSpace(path)) continue;
                context.GarantiaFotos.Add(new GarantiaFoto
                {
                    IdGarantia = idGarantia.Value,
                    VFoto = path
                });
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(path)) entity.VFoto = path;
            }
        }
    }

    private async Task ActualizarGarantiaAsync(Credito credito, GarantiaRequest? request)
    {
        if (request == null) return;

        var entity = credito.IdGarantia.HasValue
            ? await context.Garantia.FindAsync(credito.IdGarantia.Value)
            : null;

        if (entity == null)
        {
            entity = new Garantium();
            context.Garantia.Add(entity);
            await context.SaveChangesAsync();
            credito.IdGarantia = entity.IdGarantia;
        }

        entity.NTipoGarantia = request.NTipoGarantia;
        entity.NMarca = request.NMarca;
        entity.NAnio = request.NAnio;
        entity.NValor = request.NValor;
    }

    private static void ActualizarCredito(Credito entity, CreditoRequest request)
    {
        entity.NProd = request.NProd;
        entity.NSubProd = request.NSubProd;
        entity.NPrestamo = request.NPrestamo;
        entity.NPeriodo = request.NPeriodo;
        entity.NNroCuotas = request.NNroCuotas;
        entity.NMontoCuota = request.NMontoCuota;
        entity.NCobroEnAgencia = request.NCobroEnAgencia;
        entity.NAceptaTerminos = request.NAceptaTerminos;
    }
}
