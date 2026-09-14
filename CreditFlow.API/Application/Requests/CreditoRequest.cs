namespace CreditFlow.API.Application.Requests
{
    // Parámetros de [esv].[CredW_RecuperaGastosAlDesemb_2] (gasto por cuota).
    public class CreditoRequest
    {
        public decimal nPrestamo { get; set; }
        public int nMoneda { get; set; } = 1;
        public int nProd { get; set; }
        public int nSubProd { get; set; }
        public bool bRefinanciado { get; set; }
        public int nPeriodo { get; set; }
        public int nTipoCargo { get; set; }
        public int nCodLineaSecundario { get; set; }
        public int nCobroEnAgencia { get; set; }
        public int nCodCred { get; set; }
        public int nCodAge { get; set; }
        public DateTime fechaDesembolso { get; set; }
    }
}
