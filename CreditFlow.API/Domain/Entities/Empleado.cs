namespace CreditFlow.API.Domain.Entities;

public partial class Empleado
{
    public int IdEmpleado { get; set; }

    public int IdUsuario { get; set; }

    public string CDocumento { get; set; } = null!;

    public string CNombres { get; set; } = null!;

    public string CPrimerApellido { get; set; } = null!;

    public string CSegundoApellido { get; set; } = null!;

    public int NSexo { get; set; }

    public int NCodAge { get; set; }

    public string CCorreo { get; set; } = null!;

    public string CTelefono { get; set; } = null!;

    public int NEstado { get; set; }
}
