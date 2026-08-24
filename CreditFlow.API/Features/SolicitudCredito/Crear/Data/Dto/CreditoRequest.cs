namespace CreditFlow.API.Features.SolicitudCredito.Data.Dto;

public class CreditoRequest
{
    public int NProd { get; set; }

    public int NSubProd { get; set; }

    public decimal NPrestamo { get; set; }
    
    public int NCodLinea { get; set; }

    public int NPeriodo { get; set; }

    public int NNroCuotas { get; set; }

    public decimal? NMontoCuota { get; set; }

    public int? NCobroEnAgencia { get; set; }

    public int? NAceptaTerminos { get; set; }
}
