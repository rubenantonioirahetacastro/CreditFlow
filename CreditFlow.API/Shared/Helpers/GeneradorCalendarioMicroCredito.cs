using System;
using System.Collections.Generic;
using System.Linq;

namespace CreditFlow.API.Shared.Helpers
{
    // Motor de cronograma fiel a ClsCalendario_SV (VB de producción, El Salvador):
    // GeneraCalendario_MicroCredito + CuotaFechaFija_MicroCredito + CalendarioFechasCuotaFija.
    // - Cuota nivelada = PMT(tasaMensual*(1+IVA), nCuotas, monto), redondeada a 2
    //   decimales ANTES de usarse en el reparto por cuota (CuotaFechaFija_MicroCredito
    //   hace Format(..., "#0.00") sobre el resultado).
    // - El capital de cada cuota sale de la cuota nivelada menos interes e IVA de
    //   interes UNICAMENTE (capital = cuotaFija - interes - ivaInteres). El gasto y
    //   su IVA NO se descuentan de la cuota nivelada: se suman aparte, por lo que el
    //   total a pagar de la cuota crece por encima de la cuota nivelada cuando hay
    //   gasto (total = cuotaFija + gasto + ivaGasto). Confirmado contra el Excel de
    //   referencia con gasto real distinto de cero (pestana MENSUAL 12, $7.03).
    // - Interés simple sobre saldo: saldo * (tasaMensual*12/nDiasAnio) * días reales
    //   transcurridos, truncado a 2 decimales (Math.Truncate, no Round).
    // - nDiasAnio es un parámetro fijo de negocio (oSesion.nNroDiasAnio en VB; código
    //   400031 en VerNegocio, valor real de producción = 366), no los días del crédito.
    // - El IVA de interes y el IVA de gasto se calculan por separado, cada uno
    //   sobre su propio monto (ivaInteres = interes*IVA, ivaGasto = gasto*IVA).
    // - Fechas (CalendarioFechasCuotaFija): "Vencimiento" y "Cobranza" son la MISMA
    //   fecha (no hay ajuste separado). Semanal se calcula desde el origen
    //   (fechaDesembolso + 7*i); Diario/Quincenal/Mensual se ENCADENAN desde la
    //   fecha ya ajustada de la cuota anterior (+1/+15/+30 días).
    // - Ajuste de fecha: pasada de feriado, luego sábado, luego domingo, luego
    //   feriado otra vez (por si el corrimiento de sábado/domingo cayó en otro
    //   feriado). Solo para Semanal (período = 7 días): si el feriado cae viernes,
    //   se RETROCEDE al jueves en vez de avanzar (para no empujar el pago al fin
    //   de semana/semana siguiente); una vez que se empieza a retroceder, se sigue
    //   retrocediendo mientras el día anterior también sea feriado.
    // - La última cuota cancela el saldo remanente exacto.
    public static class GeneradorCalendarioMicroCredito
    {
        public record CuotaCalendario(
            int NroCuota,
            DateTime FechaVencimiento,
            DateTime FechaCobranza,
            int DiasTranscurridos,
            decimal SaldoAnterior,
            decimal Capital,
            decimal Interes,
            decimal IvaInteres,
            decimal Gasto,
            decimal IvaGasto,
            decimal TotalCuota,
            decimal SaldoDespues);

        public record ResultadoCalendario(
            decimal CuotaFija,
            decimal TasaEfectivaMensualConIva,
            int NDiasAnio,
            List<CuotaCalendario> Cuotas);

        public static ResultadoCalendario Generar(
            decimal monto,
            int nCuotas,
            int nSubProd,
            decimal tasaMensualPorcentaje,
            DateTime fechaDesembolso,
            decimal gastoPorCuota,
            int nDiasAnio,
            IEnumerable<DateTime>? feriados = null,
            decimal tasaIva = 0.13m,
            bool permiteSabado = false,
            bool permiteDomingo = false,
            bool permiteFeriado = false)
        {
            if (nDiasAnio <= 0)
                throw new ArgumentException("Los días del año deben ser mayor a cero.", nameof(nDiasAnio));
            if (monto <= 0)
                throw new ArgumentException("El monto debe ser mayor a cero.", nameof(monto));
            if (nCuotas <= 0)
                throw new ArgumentException("El número de cuotas debe ser mayor a cero.", nameof(nCuotas));

            decimal tasaMensual = tasaMensualPorcentaje / 100m;
            decimal tasaEfectivaConIva = tasaMensual * (1m + tasaIva);

            var feriadosSet = new HashSet<DateTime>((feriados ?? Enumerable.Empty<DateTime>()).Select(f => f.Date));
            bool esSemanal = nSubProd == 2;

            DateTime AjustarFeriados(DateTime fecha)
            {
                if (permiteFeriado)
                    return fecha;

                bool retro = false;
                while (feriadosSet.Contains(fecha.Date))
                {
                    if (retro)
                        fecha = fecha.AddDays(-1);
                    else if (esSemanal && fecha.DayOfWeek == DayOfWeek.Friday)
                    {
                        fecha = fecha.AddDays(-1);
                        retro = true;
                    }
                    else
                        fecha = fecha.AddDays(1);
                }
                return fecha;
            }

            DateTime AjustarFecha(DateTime fecha)
            {
                fecha = AjustarFeriados(fecha);
                if (!permiteSabado && fecha.DayOfWeek == DayOfWeek.Saturday)
                    fecha = fecha.AddDays(1);
                if (!permiteDomingo && fecha.DayOfWeek == DayOfWeek.Sunday)
                    fecha = fecha.AddDays(1);
                return AjustarFeriados(fecha);
            }

            int periodoDias = PeriodoDias(nSubProd);

            var vencimientos = new List<DateTime>(nCuotas);
            DateTime fechaAnterior = fechaDesembolso;

            for (int i = 1; i <= nCuotas; i++)
            {
                DateTime vencimiento = esSemanal
                    ? fechaDesembolso.AddDays(periodoDias * i)
                    : fechaAnterior.AddDays(periodoDias);

                vencimiento = AjustarFecha(vencimiento);

                vencimientos.Add(vencimiento);
                fechaAnterior = vencimiento;
            }

            decimal cuotaFija = Math.Round(CalcularPmt(tasaEfectivaConIva, nCuotas, monto), 2);
            decimal tasaDiariaBase = (tasaMensual * 12m) / nDiasAnio;

            var cuotas = new List<CuotaCalendario>(nCuotas);
            decimal saldo = monto;
            DateTime fechaPrevia = fechaDesembolso;

            for (int i = 1; i <= nCuotas; i++)
            {
                DateTime fechaVenc = vencimientos[i - 1];
                int dias = (fechaVenc.Date - fechaPrevia.Date).Days;
                decimal saldoAnterior = saldo;

                decimal interes = Truncar2(saldoAnterior * tasaDiariaBase * dias);
                decimal gasto = Math.Round(gastoPorCuota, 2);
                decimal ivaInteres = Math.Round(interes * tasaIva, 2);
                decimal ivaGasto = Math.Round(gasto * tasaIva, 2);

                // El capital sale de la cuota nivelada descontando solo interes e IVA
                // de interes. El gasto y su IVA se suman aparte (no reducen capital):
                // el total de la cuota crece por encima de la cuota nivelada cuando
                // hay gasto. Confirmado contra el Excel de referencia con gasto real
                // distinto de cero (pestana MENSUAL 12, $7.03 de gasto por cuota).
                decimal capital = i == nCuotas
                    ? saldoAnterior
                    : Math.Round(cuotaFija - interes - ivaInteres, 2);

                decimal totalCuota = Math.Round(capital + interes + ivaInteres + gasto + ivaGasto, 2);
                decimal saldoDespues = Math.Round(saldoAnterior - capital, 2);

                cuotas.Add(new CuotaCalendario(
                    i,
                    fechaVenc,
                    fechaVenc,
                    dias,
                    Math.Round(saldoAnterior, 2),
                    capital,
                    interes,
                    ivaInteres,
                    gasto,
                    ivaGasto,
                    totalCuota,
                    saldoDespues));

                saldo = saldoDespues;
                fechaPrevia = fechaVenc;
            }

            return new ResultadoCalendario(cuotaFija, tasaEfectivaConIva, nDiasAnio, cuotas);
        }

        private static decimal CalcularPmt(decimal tasaPeriodica, int nPeriodos, decimal valorPresente)
        {
            if (tasaPeriodica == 0m)
                return valorPresente / nPeriodos;

            double r = (double)tasaPeriodica;
            double factor = Math.Pow(1 + r, nPeriodos);
            return valorPresente * (decimal)(r * factor / (factor - 1));
        }

        private static decimal Truncar2(decimal valor) => Math.Truncate(valor * 100m) / 100m;

        // Días nominales del período por subproducto (nSubProd, catálogo 109):
        // 1=Diario, 2=Semanal, 3=Quincenal, 4=Mensual. Coincide con el nPeriodo que
        // exige CredGastos/CredW_RecuperaGastosAlDesemb_2 (30/7/15/1 días).
        public static int PeriodoDias(int nSubProd) => nSubProd switch
        {
            1 => 1,
            2 => 7,
            3 => 15,
            4 => 30,
            _ => throw new ArgumentException($"nSubProd {nSubProd} no está definido para Crédito Microempresa.")
        };
    }
}
