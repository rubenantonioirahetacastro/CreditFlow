namespace CreditFlow.API.Features.Credit.Shared.Contracts;

public class GarantiaRequest
{
    public int IdGarantia { get; set; }
    public int NTipoGarantia { get; set; }
    public int NMarca { get; set; }
    public int NAnio { get; set; }
    public decimal NValor { get; set; }
}
