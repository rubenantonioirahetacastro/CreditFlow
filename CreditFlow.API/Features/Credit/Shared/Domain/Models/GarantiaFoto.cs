namespace CreditFlow.API.Features.Credit.Shared.Domain.Models;

public partial class GarantiaFoto
{
    public int IdFoto { get; set; }

    public string VFoto { get; set; } = null!;

    public int IdGarantia { get; set; }
}
