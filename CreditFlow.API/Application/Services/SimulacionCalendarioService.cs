using CreditFlow.API.Application.Interfaces;
using CreditFlow.API.Shared.Helpers;
using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Infrastructure.Data;
using CreditFlow.API.Application.Requests;
using CreditFlow.API.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Application.Services
{
    public class SimulacionCalendarioService : ISimulacionCalendarioService
    {
        // Códigos de VerNegocio (equivalente a DevuelveVarNegocio() en el VB de
        // producción). Confirmados contra el dump real de VarNegocio de producción.
        private const int COD_VALOR_IGV = 36;
        private const int COD_NRO_DIA_ANIO = 400031; // oSesion.nNroDiasAnio (= 366 en producción)

        private readonly DbNegocioContext _context;
        private readonly IPrimLineaCreditoService _primLineaCreditoService;
        private readonly IGastoService _gastoService;
        private readonly IFeriadoService _feriadoService;
        private readonly IVarNegocioService _varNegocioService;

        public SimulacionCalendarioService(
            DbNegocioContext context,
            IPrimLineaCreditoService primLineaCreditoService,
            IGastoService gastoService,
            IFeriadoService feriadoService,
            IVarNegocioService varNegocioService)
        {
            _context = context;
            _primLineaCreditoService = primLineaCreditoService;
            _gastoService = gastoService;
            _feriadoService = feriadoService;
            _varNegocioService = varNegocioService;
        }

        public async Task<SimularCalendarioResponse> SimularAsync(SimularCalendarioRequest request)
        {
            if (request.Monto <= 0)
                throw new InvalidOperationException("El monto solicitado debe ser mayor a 0.");

            if (request.NPlazo <= 0)
                throw new InvalidOperationException("El plazo debe ser mayor a 0.");

            if (request.NCodAge <= 0)
                throw new InvalidOperationException("La agencia (NCodAge) es obligatoria para resolver la línea de crédito.");

            DateTime fechaInicio = request.FechaInicio ?? DateTime.Today;
            int periodoDias = GeneradorCalendarioMicroCredito.PeriodoDias(request.NSubProd);

            var primLinea = await _primLineaCreditoService.ObtenerPrimLineaCredAsync(new PrimLineaCreditoRequest
            {
                NCodAge = request.NCodAge,
                NCuotas = request.NPlazo,
                NMonto = request.Monto,
                NNumPrestamo = request.NNumPrestamo,
                NMoneda = request.NMoneda,
                NProd = request.NProd,
                NSubProd = request.NSubProd,
                NCodCamp = request.NCodCamp,
                NCategoria = request.NCategoria,
                BRefinanciado = request.BRefinanciado,
                BCustodia = request.BCustodia,
                NPeriodo = periodoDias,
                NCodCred = 0
            });

            if (primLinea.NCodLinea == 0)
                throw new InvalidOperationException("No existe una línea de crédito configurada para la agencia, producto, subproducto, plazo, monto, moneda y campaña indicados.");

            decimal tasaNominalMensual = request.TasaOverride ?? primLinea.NTasaCom;

            decimal gastoPorCuota = request.GastoOverride ?? await _gastoService.ObtenerGastoAsync(new CreditoRequest
            {
                nPrestamo = request.Monto,
                nMoneda = request.NMoneda,
                nProd = request.NProd,
                nSubProd = request.NSubProd,
                bRefinanciado = request.BRefinanciado,
                nPeriodo = periodoDias,
                nTipoCargo = request.NTipoCargo,
                nCodLineaSecundario = request.NCodLineaSecundario,
                nCobroEnAgencia = 0,
                nCodCred = 0,
                nCodAge = request.NCodAge,
                fechaDesembolso = fechaInicio
            });

            var feriados = request.NCodAge > 0
                ? await _feriadoService.ObtenerFeriadosAsync(fechaInicio, request.NCodAge)
                : new List<DateTime>();

            decimal tasaIva = await _varNegocioService.ObtenerValorDecimalAsync(COD_VALOR_IGV, 0.13m);
            int nDiasAnio = await _varNegocioService.ObtenerValorIntAsync(COD_NRO_DIA_ANIO, 366);

            var resultado = GeneradorCalendarioMicroCredito.Generar(
                request.Monto,
                request.NPlazo,
                request.NSubProd,
                tasaNominalMensual,
                fechaInicio,
                gastoPorCuota,
                nDiasAnio,
                feriados,
                tasaIva,
                request.PermiteSabado,
                request.PermiteDomingo,
                request.PermiteFeriado);

            var cronograma = resultado.Cuotas.Select(c => new CuotaDetalleResponse
            {
                NroCuota = c.NroCuota,
                FechaVencimiento = c.FechaVencimiento,
                FechaCobranza = c.FechaCobranza,
                Capital = c.Capital,
                Interes = c.Interes,
                Gasto = c.Gasto,
                Iva = c.IvaInteres + c.IvaGasto,
                TotalCuota = c.TotalCuota,
                SaldoDespues = c.SaldoDespues
            }).ToList();

            var flujosParaTir = resultado.Cuotas
                .Select(c => new CredCalendario { DFecVenc = c.FechaVencimiento, NTotalCuota = c.TotalCuota })
                .ToList();
            decimal teaReal = CalculadoraFinanciera.CalcularTeaPorTir(request.Monto, flujosParaTir, fechaInicio);

            var (teaMaximaLegal, segmentoLegal, cumpleLeyUsura) = await ObtenerEvaluacionUsuraAsync(request.Monto, fechaInicio, teaReal);

            decimal totalCapital = cronograma.Sum(c => c.Capital);
            decimal totalInteres = cronograma.Sum(c => c.Interes);
            decimal totalGasto = cronograma.Sum(c => c.Gasto);
            decimal totalIva = cronograma.Sum(c => c.Iva);
            decimal totalPagado = cronograma.Sum(c => c.TotalCuota);

            return new SimularCalendarioResponse
            {
                LineaUsada = primLinea.CDescLinea,
                TasaNominalMensual = tasaNominalMensual,
                CuotaFija = resultado.CuotaFija,
                MontoSolicitado = request.Monto,
                Plazo = request.NPlazo,
                Cronograma = cronograma,
                TotalCapital = totalCapital,
                TotalInteres = totalInteres,
                TotalGasto = totalGasto,
                TotalIva = totalIva,
                TotalPagado = totalPagado,
                CostoTotalCredito = Math.Round(totalPagado - request.Monto, 2),
                TeaReal = teaReal,
                TeaMaximaLegal = teaMaximaLegal,
                SegmentoLegal = segmentoLegal,
                CumpleLeyUsura = cumpleLeyUsura
            };
        }

        private async Task<(decimal? TeaMaximaLegal, string? SegmentoLegal, bool CumpleLeyUsura)> ObtenerEvaluacionUsuraAsync(decimal montoCredito, DateTime fechaCredito, decimal teaReal)
        {
            var fechaConsulta = DateOnly.FromDateTime(fechaCredito);

            var salarioMinimo = await _context.SalarioMinimoVigentes
                .Where(s => s.BEstado && fechaConsulta >= s.DFecInicio && (s.DFecFin == null || fechaConsulta <= s.DFecFin))
                .OrderByDescending(s => s.DFecInicio)
                .FirstOrDefaultAsync();

            if (salarioMinimo is null || salarioMinimo.NMontoMensual <= 0)
                return (null, null, false);

            decimal montoEnSalarios = montoCredito / salarioMinimo.NMontoMensual;

            var segmento = await _context.CatSegmentoUsuras
                .Where(s => s.BEstado && montoEnSalarios > s.NMultSmmin && (s.NMultSmmax == null || montoEnSalarios <= s.NMultSmmax))
                .FirstOrDefaultAsync();

            TasaMaximaBcr? tasaMaxima = null;

            if (segmento is null)
            {
                tasaMaxima = await _context.TasaMaximaBcrs
                    .Where(t => t.BEstado && fechaConsulta >= t.DFecInicio && fechaConsulta <= t.DFecFin)
                    .OrderByDescending(t => t.NTasaMaxima)
                    .FirstOrDefaultAsync();
            }
            else
            {
                tasaMaxima = await _context.TasaMaximaBcrs
                    .Where(t => t.BEstado && t.NCodSegmento == segmento.NCodSegmento && fechaConsulta >= t.DFecInicio && fechaConsulta <= t.DFecFin)
                    .FirstOrDefaultAsync();
            }

            if (tasaMaxima is null)
                return (null, segmento?.CDescripcion, false);

            return (tasaMaxima.NTasaMaxima, segmento?.CDescripcion ?? "Segmento general", teaReal <= tasaMaxima.NTasaMaxima);
        }
    }
}
