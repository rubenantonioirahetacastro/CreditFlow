using CreditFlow.API.Core.Finance;
using CreditFlow.API.Core.Errors;
using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Features.Credit.Shared.Calendar;
using CreditFlow.API.Features.Credit.Shared.CreditLine;
using CreditFlow.API.Features.Simulator.Shared.Errors;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Simulator.Shared.SimulateCalendar
{
    public class SimulacionCalendarioService : ISimulacionCalendarioService
    {
        // Códigos de VerNegocio (equivalente a DevuelveVarNegocio() en el VB de
        // producción). Confirmados contra el dump real de VarNegocio de producción.
        private const int COD_VALOR_IGV = 36;
        private const int COD_NRO_DIA_ANIO = 400031; // oSesion.nNroDiasAnio (= 366 en producción)

        private readonly DbNegocioContext _context;
        private readonly IPrimaryCreditLineService _primaryCreditLineService;
        private readonly IExpenseService _expenseService;
        private readonly IHolidayService _holidayService;
        private readonly IBusinessVariableService _businessVariableService;

        public SimulacionCalendarioService(
            DbNegocioContext context,
            IPrimaryCreditLineService primaryCreditLineService,
            IExpenseService expenseService,
            IHolidayService holidayService,
            IBusinessVariableService businessVariableService)
        {
            _context = context;
            _primaryCreditLineService = primaryCreditLineService;
            _expenseService = expenseService;
            _holidayService = holidayService;
            _businessVariableService = businessVariableService;
        }

        public async Task<SimularCalendarioResponse> SimularAsync(SimularCalendarioRequest request)
        {
            if (request.Monto <= 0)
                throw new RequestValidationException(SimulatorErrors.InvalidAmount);

            if (request.NPlazo <= 0)
                throw new RequestValidationException(SimulatorErrors.InvalidTerm);

            if (request.NCodAge <= 0)
                throw new RequestValidationException(SimulatorErrors.AgencyRequired);

            DateTime fechaInicio = request.FechaInicio ?? DateTime.Today;
            int periodoDias = MicrocreditCalendarGenerator.GetPeriodDays(request.NSubProd);

            PrimaryCreditLineResult primLinea;
            if (request.NCodLinea > 0)
            {
                primLinea = await _context.CredLineaCreditos
                    .AsNoTracking()
                    .Where(line => line.NCodLinea == request.NCodLinea)
                    .Select(line => new PrimaryCreditLineResult(
                        line.NCodLinea,
                        line.CDescripcion ?? string.Empty,
                        line.NTasaCom,
                        0m))
                    .FirstOrDefaultAsync()
                    ?? throw new ResourceNotFoundException(SimulatorErrors.AssignedCreditLineNotFound);
            }
            else
            {
                primLinea = await _primaryCreditLineService.ResolveAsync(new PrimaryCreditLineRequest
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
            }

            if (primLinea.NCodLinea == 0)
                throw new BusinessRuleException(SimulatorErrors.CreditLineNotConfigured);

            decimal tasaNominalMensual = request.TasaOverride ?? primLinea.NTasaCom;

            decimal gastoPorCuota = request.GastoOverride ?? await _expenseService.GetExpenseAsync(new ExpenseRequest
            {
                Amount = request.Monto,
                Currency = request.NMoneda,
                Product = request.NProd,
                SubProduct = request.NSubProd,
                IsRefinanced = request.BRefinanciado,
                Period = periodoDias,
                ChargeType = request.NTipoCargo,
                SecondaryCreditLineCode = request.NCodLineaSecundario,
                CollectAtAgency = 0,
                CreditCode = 0,
                AgencyCode = request.NCodAge,
                DisbursementDate = fechaInicio
            });

            var feriados = request.NCodAge > 0
                ? await _holidayService.GetHolidaysAsync(fechaInicio, request.NCodAge)
                : new List<DateTime>();

            decimal tasaIva = await _businessVariableService.GetDecimalAsync(COD_VALOR_IGV, 0.13m);
            int nDiasAnio = await _businessVariableService.GetIntAsync(COD_NRO_DIA_ANIO, 366);

            var resultado = MicrocreditCalendarGenerator.Generate(
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
