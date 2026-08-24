namespace CreditFlow.API.Features.SolicitudCredito.Data.Dto;

public class VentaRequest
{
    public string CProducto { get; set; } = null!;

    public decimal NCantidadVenta { get; set; }

    public int NUnidadMedida { get; set; }

    public decimal NPrecioXunidad { get; set; }

    public decimal NPrecioTotal { get; set; }
}
