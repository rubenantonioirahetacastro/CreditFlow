namespace CreditFlow.API.Features.Credit.Shared.Contracts;

public class CompraRequest
{
    public string CProducto { get; set; } = null!;

    public decimal NCantidadCompra { get; set; }

    public int NUnidadMedida { get; set; }

    public decimal NPrecioXunidad { get; set; }

    public decimal NPrecioTotal { get; set; }
}
