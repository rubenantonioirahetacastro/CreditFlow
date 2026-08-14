namespace CreditFlow.Web.Features.Mantenimientos.Roles.Models;

public class ActualizarRoleRequest
{
    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public bool Activo { get; set; }
}
