namespace CreditFlow.API.Features.Agency.Web.ManageAgencies;

public class AgencyDto
{
    public int NCodAge { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string CorreoElectronico { get; set; } = string.Empty;
}
