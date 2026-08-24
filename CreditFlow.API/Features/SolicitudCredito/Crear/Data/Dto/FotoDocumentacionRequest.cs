using Microsoft.AspNetCore.Http;

namespace CreditFlow.API.Features.SolicitudCredito.Data.Dto;

public class FotoDocumentacionRequest
{
    public int IdFoto { get; set; }
    public string? VFoto { get; set; }
    public int IdTipoDocumentacion { get; set; }
    public IFormFile? Archivo { get; set; }
}
