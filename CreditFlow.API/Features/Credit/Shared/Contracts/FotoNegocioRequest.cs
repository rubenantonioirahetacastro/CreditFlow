namespace CreditFlow.API.Features.Credit.Shared.Contracts;

public class FotoNegocioRequest
{
    public int IdFoto { get; set; }
    public string? VFoto { get; set; }
    public int NTipoFoto { get; set; }
    public IFormFile? Archivo { get; set; }
}
