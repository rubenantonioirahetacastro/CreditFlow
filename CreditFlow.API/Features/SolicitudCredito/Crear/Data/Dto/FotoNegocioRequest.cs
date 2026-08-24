namespace CreditFlow.API.Features.SolicitudCredito.Data.Dto;

public class FotoNegocioRequest
{
    public int IdFoto { get; set; }
    public string? VFoto { get; set; }
    public int NTipoFoto { get; set; }
    public IFormFile? Archivo { get; set; }
}
