namespace CreditFlow.API.Features.Credit.Mobile.GetCreditDetail;

public class CapacidadPagoDetalleDto
{
    public int IdCapacidadPago { get; set; }
    public decimal DGastosEducacion { get; set; }
    public decimal DGastosAlimentacion { get; set; }
    public decimal DGastosSalud { get; set; }
    public decimal DOtrosGastos { get; set; }
    public decimal DOtrosIngresos { get; set; }
}
