namespace CreditFlow.API.Features.SolicitudCredito.Buscar.Dto;

public class NegocioDetalleDto
{
    public string CNombre { get; set; } = string.Empty;
    public string CDireccion { get; set; } = string.Empty;
    public int CSector { get; set; }
    public TimeOnly? THoraInicio { get; set; }
    public TimeOnly? THoraCierre { get; set; }
    public string CTelefono { get; set; } = string.Empty;
    public string? CGeolocalizacion { get; set; }
    public List<VentaDetalleDto> Ventas { get; set; } = new();
    public List<CompraDetalleDto> Compras { get; set; } = new();
    public List<FotoDto> Fotos { get; set; } = new();
}
