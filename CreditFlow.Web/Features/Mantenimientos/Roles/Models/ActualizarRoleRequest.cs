namespace CreditFlow.Web.Models.Mantenimientos;

public class ActualizarRoleRequest
{
    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public bool Activo { get; set; }
}
