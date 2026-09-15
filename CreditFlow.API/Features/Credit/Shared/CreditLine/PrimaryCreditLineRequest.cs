namespace CreditFlow.API.Features.Credit.Shared.CreditLine
{
    // Parámetros de [ESV].[ObtenerPrimLineaCred].
    public class PrimaryCreditLineRequest
    {
        public int NCodAge { get; set; }

        public int NCuotas { get; set; }

        public decimal NMonto { get; set; }

        public int NNumPrestamo { get; set; }

        public int NMoneda { get; set; } = 1;

        public int NProd { get; set; }

        public int NSubProd { get; set; }

        public int NCodCamp { get; set; }

        public int NCategoria { get; set; }

        public bool BRefinanciado { get; set; }

        public bool BCustodia { get; set; }

        // -1 = ignora el filtro de período al buscar la tasa de comisión.
        public int NPeriodo { get; set; } = -1;

        public int NCodCred { get; set; }
    }
}
