namespace CreditFlow.API.Features.Credit.Shared.Contracts;

public class CapacidadPagoRequest
{
    public decimal DGastosEducacion { get; set; }

    public decimal DGastosAlimentacion { get; set; }

    public decimal DGastosSalud { get; set; }

    public decimal DOtrosGastos { get; set; }

    public decimal DOtrosIngresos { get; set; }
}
