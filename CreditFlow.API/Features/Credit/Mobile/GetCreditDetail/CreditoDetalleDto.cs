namespace CreditFlow.API.Features.Credit.Mobile.GetCreditDetail;

public class CreditoDetalleDto
{
    public int NCodAge { get; set; }
    public int NCodCred { get; set; }
    public int NProd { get; set; }
    public int NSubProd { get; set; }
    public decimal NPrestamo { get; set; }
    public decimal NSaldoK { get; set; }
    public int? NCodLinea { get; set; }
    public int NEstado { get; set; }
    public string? EstadoNombre { get; set; }
    public int NPeriodo { get; set; }
    public int NNroCuotas { get; set; }
    public int? NCobroEnAgencia { get; set; }
    public int? NAceptaTerminos { get; set; }
    public DateTime DFecVig { get; set; }
}
