namespace CreditFlow.API.Features.SolicitudCredito.Buscar.Dto;

public class CapacidadPagoDetalleDto
{
    public decimal DGastosEducacion { get; set; }
    public decimal DGastosAlimentacion { get; set; }
    public decimal DGastosSalud { get; set; }
    public decimal DOtrosGastos { get; set; }
    public decimal DOtrosIngresos { get; set; }
}
