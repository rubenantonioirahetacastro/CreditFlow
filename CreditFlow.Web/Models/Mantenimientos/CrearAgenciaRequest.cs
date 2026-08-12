namespace CreditFlow.Web.Models.Mantenimientos;

// NCodAge se ingresa manualmente: en la API no es autogenerado por la base de datos.
public class CrearAgenciaRequest
{
    public int NCodAge { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Direccion { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;

    public string CorreoElectronico { get; set; } = string.Empty;
}
