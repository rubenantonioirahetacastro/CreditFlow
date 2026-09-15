namespace CreditFlow.API.Features.Credit.Mobile.GetCreditDetail;

public class NegocioDetalleDto
{
    public int IdNegocio { get; set; }
    public string CNombre { get; set; } = string.Empty;
    public string CDireccion { get; set; } = string.Empty;
    public int CSector { get; set; }
    public string? THoraInicio { get; set; }
    public string? THoraCierre { get; set; }
    public string CTelefono { get; set; } = string.Empty;
    public string? CGeolocalizacion { get; set; }
    public List<VentaDetalleDto> Ventas { get; set; } = new();
    public List<CompraDetalleDto> Compras { get; set; } = new();
    public List<FotoDto> Fotos { get; set; } = new();
}
