namespace CreditFlow.API.Features.Credit.Shared.Contracts;

public class GarantiaFotoRequest
{
    public int IdFoto { get; set; }
    public string? VFoto { get; set; }
    public IFormFile? Archivo { get; set; }
}
