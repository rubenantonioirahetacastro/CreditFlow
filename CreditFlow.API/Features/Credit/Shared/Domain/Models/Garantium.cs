namespace CreditFlow.API.Features.Credit.Shared.Domain.Models;

public partial class Garantium
{
    public int IdGarantia { get; set; }

    public int NTipoGarantia { get; set; }

    public int NMarca { get; set; }

    public int NAnio { get; set; }

    public decimal NValor { get; set; }
}
