namespace CreditFlow.API.Features.Credit.Mobile.GetCreditDetail;

public class DocumentacionDetalleDto
{
    public int IdDocumentacion { get; set; }
    public List<FotoDto> Fotos { get; set; } = new();
}
