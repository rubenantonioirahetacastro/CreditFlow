namespace CreditFlow.Web.Models;

public class SimularCalendarioRequest
{
    public int NProd { get; set; } = 4;

    public int NSubProd { get; set; }

    public int NPlazo { get; set; }

    public decimal Monto { get; set; }

    public DateTime? FechaInicio { get; set; }

    public decimal? TasaOverride { get; set; }

    public decimal? GastoOverride { get; set; }

    public int NCodAge { get; set; }

    public int NMoneda { get; set; } = 1;

    public int NCodCamp { get; set; }

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
