namespace CreditFlow.API.Features.ClientesMovil.Dto;

public class ClienteMovilDto
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
