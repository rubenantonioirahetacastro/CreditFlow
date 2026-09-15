namespace CreditFlow.API.Features.Credit.Shared.Contracts;

public class VentaRequest
{
    public string CProducto { get; set; } = null!;

    public decimal NCantidadVenta { get; set; }

    public int NUnidadMedida { get; set; }

    public decimal NPrecioXunidad { get; set; }

    public decimal NPrecioTotal { get; set; }
}
