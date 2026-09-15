namespace CreditFlow.API.Features.Credit.Mobile.GetCreditDetail;

public class CompraDetalleDto
{
    public string CProducto { get; set; } = string.Empty;
    public decimal NCantidadCompra { get; set; }
    public int NUnidadMedida { get; set; }
    public decimal NPrecioXunidad { get; set; }
    public decimal NPrecioTotal { get; set; }
}
