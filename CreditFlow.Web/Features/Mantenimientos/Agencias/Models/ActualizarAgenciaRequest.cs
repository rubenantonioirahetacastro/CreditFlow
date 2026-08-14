namespace CreditFlow.Web.Features.Mantenimientos.Agencias.Models;

public class ActualizarAgenciaRequest
{
    public string Nombre { get; set; } = string.Empty;

    public string Direccion { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;

    public string CorreoElectronico { get; set; } = string.Empty;
}
