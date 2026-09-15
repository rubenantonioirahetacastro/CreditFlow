namespace CreditFlow.API.Features.Credit.Mobile.GetCreditDetail;

public class VentaDetalleDto
{
    public string CProducto { get; set; } = string.Empty;
    public decimal NCantidadVenta { get; set; }
    public int NUnidadMedida { get; set; }
    public decimal NPrecioXunidad { get; set; }
    public decimal NPrecioTotal { get; set; }
}
