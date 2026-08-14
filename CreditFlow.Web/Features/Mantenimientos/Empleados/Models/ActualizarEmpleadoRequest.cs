using Microsoft.AspNetCore.Components.Forms;

namespace CreditFlow.Web.Models.Mantenimientos;

public class ActualizarEmpleadoRequest
{
    public string Nombres { get; set; } = string.Empty;

    public string PrimerApellido { get; set; } = string.Empty;

    public string SegundoApellido { get; set; } = string.Empty;

    public int Sexo { get; set; }

    public int CodAgencia { get; set; }

    public string Correo { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;

    public int Estado { get; set; }

    public int IdRol { get; set; }

    public IBrowserFile? Foto { get; set; }
}
