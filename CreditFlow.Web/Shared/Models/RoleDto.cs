namespace CreditFlow.Web.Shared.Models;

public class RoleDto
{
    public int IdRol { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public bool Activo { get; set; }
}
