namespace CreditFlow.API.Features.Verification.Mobile.GetClients;

public class ClientListVerifierDto
{
    public int NCodAge { get; set; }
    public int NCodCred { get; set; }
    public int? IdPersona { get; set; }
    public string? Nombre { get; set; }
    public string? NombreNegocio { get; set; }
    public string? Documento { get; set; }
    public string? Telefono { get; set; }
    public string? DireccionNegocio { get; set; }
    public string? GeolocalizacionNegocio { get; set; }
}
