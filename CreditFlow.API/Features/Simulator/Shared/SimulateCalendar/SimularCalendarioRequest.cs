using System.ComponentModel.DataAnnotations;

namespace CreditFlow.API.Features.Simulator.Shared.SimulateCalendar
{
    public class SimularCalendarioRequest
    {
        public int NProd { get; set; }

        public int NSubProd { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El plazo debe ser mayor que cero.")]
        public int NPlazo { get; set; }

        [Range(typeof(decimal), "0.01", "999999999", ErrorMessage = "El monto solicitado debe ser mayor que cero.")]
        public decimal Monto { get; set; }

        public DateTime? FechaInicio { get; set; }

        public decimal? TasaOverride { get; set; }

        public decimal? GastoOverride { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La agencia es obligatoria.")]
        public int NCodAge { get; set; }

        // 1 = moneda nacional, 2 = moneda extranjera (igual que CredLineaCredito/CredGastos).
        public int NMoneda { get; set; } = 1;

        public int NCodCamp { get; set; }

        public int NCodLinea { get; set; }

        public int NCategoria { get; set; }

        public bool BRefinanciado { get; set; }

        public bool BCustodia { get; set; }

        public int NNumPrestamo { get; set; } = 1;

        public int NTipoCargo { get; set; }

        public int NCodLineaSecundario { get; set; }

        public bool PermiteSabado { get; set; }

        public bool PermiteDomingo { get; set; }

        public bool PermiteFeriado { get; set; }
    }
}
