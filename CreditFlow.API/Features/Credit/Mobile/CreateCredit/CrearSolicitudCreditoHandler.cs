using CreditFlow.API.Core.Diagnostics;
using CreditFlow.API.Core.Email;
using CreditFlow.API.Core.Security;
using CreditFlow.API.Core.Errors;
using CreditFlow.API.Features.Credit.Shared.Calendar;
using CreditFlow.API.Features.Credit.Shared.CreditLine;
using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Features.Credit.Shared.Domain.Models;
using CreditFlow.API.Features.Credit.Shared.Errors;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace CreditFlow.API.Features.Credit.Mobile.CreateCredit
{
    public class CrearSolicitudCreditoHandler(
        DbNegocioContext context,
        IErrorLogger errorLogger,
        IEmailService emailService,
        ICalendarioService calendarioService,
        ILineaCreditoService lineaCreditoService) : ICrearSolicitudCreditoHandler
    {
        public async Task<int> EjecutarAsync(
            List<FotoId>? fotoId,
            List<FotoDocumentacion>? fotoDocumentacion,
            List<GarantiaFoto>? fotoGarantia,
            List<FotoNegocio>? fotoNegocio,
            Garantium garantia,
            Persona persona,
            Conyuge? conyuge,
            Fiador? fiador,
            Negocio? negocio,
            CapacidadPago? capacidadPago,
            List<Compra>? compra,
            List<Venta>? venta,
            Credito credito,
            string? fotoPerfil = null)
        {
            var solicitudExistente = await context.Personas
                .AsNoTracking()
                .AnyAsync(p => p.CDocumento == persona.CDocumento);

            if (solicitudExistente)
                throw new ResourceConflictException(CreditErrors.RequestAlreadyExists(persona.CDocumento));

            var lineaCredito = await lineaCreditoService.ResolverLineaCreditoAsync(credito.NSubProd, credito.NPrestamo);

            if (credito.NNroCuotas < lineaCredito.NPlazoMin || credito.NNroCuotas > lineaCredito.NPlazoMax)
                throw new BusinessRuleException(CreditErrors.TermOutsideCreditLine(
                    credito.NNroCuotas,
                    lineaCredito.NPlazoMin,
                    lineaCredito.NPlazoMax));

            credito.NCodLinea = lineaCredito.NCodLinea;
            credito.NTasaComp = lineaCredito.NTasaCom;
            credito.DFecVig = DateTime.Now;
            credito.NEstado = 1; // Solicitado (catálogo 116)


            await using var tx = await context.Database.BeginTransactionAsync();

            try
            {
                var documentacion = new Documentacion();
                // UsuarioLogin no conoce a Persona: la relación es Persona -> UsuarioLogin
                // (Persona.IdUsuario referencia a UsuarioLogin.IdUsuario). Por eso el login
                // se crea y guarda primero, para poder asignar Persona.IdUsuario antes de
                // guardar la Persona.
                var correo = string.IsNullOrWhiteSpace(persona.CCorreo)
                    ? $"{persona.CDocumento}@crediavanza.com"
                    : persona.CCorreo;
                const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
                var random = new Random();

                var passwordTemporal = new string(
                    Enumerable.Repeat(chars, 6)
                        .Select(s => s[random.Next(s.Length)])
                        .ToArray()
                );
                var token = Random.Shared.Next(0, 1_000_000).ToString("D6");
                var tokenInt = int.Parse(token);

                var usuario = new UsuarioLogin
                {
                    CDocumento = persona.CDocumento,
                    CCodUsu = string.Empty,
                    VFoto = fotoPerfil,
                    CCorreo = correo,
                    Password = BCrypt.Net.BCrypt.HashPassword(passwordTemporal),
                    Token = tokenInt,
                    TokenTime = DateTime.UtcNow,
                    Estado = 1,
                    IntentosFallidos = 0,
                    Bloqueado = 0,
                    TokenCheck = false,
                    BContrasenaTemporal = true,
                    DFechaContrasenaTemporal = DateTime.UtcNow
                };

                await context.UsuarioLogins.AddAsync(usuario);
                await context.SaveChangesAsync();
                
                var role = await context.Roles
                    .FirstOrDefaultAsync(item =>
                        item.IdRol == RoleIds.Client && item.Activo)
                    ?? throw new BusinessRuleException(CreditErrors.ClientRoleUnavailable);

                var usuarioRole = new UsuarioRole
                {
                    IdUsuario = usuario.IdUsuario,
                    IdRol = role.IdRol,
                    FechaAsignacion = DateTime.UtcNow
                };

                await context.UsuarioRoles.AddAsync(usuarioRole);

                persona.IdUsuario = usuario.IdUsuario;

                await context.Personas.AddAsync(persona);

                if (conyuge != null) await context.Conyuges.AddAsync(conyuge);
                if (fiador != null) await context.Fiadors.AddAsync(fiador);
                if (negocio != null) await context.Negocios.AddAsync(negocio);

                await context.Documentacions.AddAsync(documentacion);
                await context.Garantia.AddAsync(garantia);
                if (capacidadPago != null) await context.CapacidadPagos.AddAsync(capacidadPago);

                await context.SaveChangesAsync();

                compra?.ForEach(x => x.IdNegocio = negocio!.IdNegocio);
                venta?.ForEach(x => x.IdNegocio = negocio!.IdNegocio);

                fotoId?.ForEach(x => x.IdPersona = persona.IdPersona);
                fotoDocumentacion?.ForEach(x => x.IdDocumentacion = documentacion.IdDocumentacion);
                fotoGarantia?.ForEach(x => x.IdGarantia = garantia.IdGarantia);
                fotoNegocio?.ForEach(x => x.IdNegocio = negocio!.IdNegocio);

                if (compra?.Any() == true) await context.Compras.AddRangeAsync(compra);
                if (venta?.Any() == true) await context.Ventas.AddRangeAsync(venta);
                if (fotoId?.Any() == true) await context.FotoIds.AddRangeAsync(fotoId);
                if (fotoDocumentacion?.Any() == true) await context.FotoDocumentacions.AddRangeAsync(fotoDocumentacion);
                if (fotoGarantia?.Any() == true) await context.GarantiaFotos.AddRangeAsync(fotoGarantia);
                if (fotoNegocio?.Any() == true) await context.FotoNegocios.AddRangeAsync(fotoNegocio);

                credito.IdPersona = persona.IdPersona;
                credito.IdConyuge = conyuge?.IdConyuge;
                credito.IdFiador = fiador?.IdFiador;
                credito.IdNegocio = negocio?.IdNegocio;
                credito.IdDocumentacion = documentacion.IdDocumentacion;
                credito.IdGarantia = garantia.IdGarantia;
                credito.IdCapacidadPago = capacidadPago?.IdCapacidadPago;

                var calendarioCond = new CredCalendCond
                {
                    NDiaFijo = 1,
                    NCuotas = credito.NNroCuotas,
                    NPlazo = credito.NPeriodo,
                    NNroCalen = 1,
                    BCobroSab = true,
                    BCobroDom = false,
                    BCobroFer = false,
                    BCuotaDoble = false,
                    IdCalenGasto = 1
                };

                await context.CredCalendConds.AddAsync(calendarioCond);
                await context.SaveChangesAsync();
                credito.IdCredCalendCond = calendarioCond.IdCredCalendCond;

                await context.Creditos.AddAsync(credito);

                int result = await context.SaveChangesAsync();

                await calendarioService.GenerarCalendarioAsync(credito.NCodAge, credito.NCodCred);

                await tx.CommitAsync();

                var subject = "Solicitud de crédito recibida";
                var nombre = string.IsNullOrWhiteSpace(persona.CNombres + ' ' + persona.CPrimerApellido)
                    ? persona.CDocumento
                    : persona.CNombres;
                var body = CreditEmailTemplates.SolicitudCredito(nombre, tokenInt, passwordTemporal);

                try
                {
                    await emailService.SendAsync(correo, subject, body);
                }
                catch (Exception ex)
                {
                    await errorLogger.LogAsync(ex);
                }

                return result;
            }
            catch (AppException)
            {
                await tx.RollbackAsync();
                throw;
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                await errorLogger.LogAsync(ex);
                throw;
            }
        }
    }
}
