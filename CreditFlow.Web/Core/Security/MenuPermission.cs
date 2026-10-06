namespace CreditFlow.Web.Core.Security;

/// <summary>Permiso sobre una opción del menú (mismo contrato que la API: api/roles/…/permisos).</summary>
public sealed class MenuPermission
{
    public string Clave { get; set; } = string.Empty;

    public bool Ver { get; set; }

    public bool Crear { get; set; }

    public bool Editar { get; set; }

    public bool Eliminar { get; set; }
}
