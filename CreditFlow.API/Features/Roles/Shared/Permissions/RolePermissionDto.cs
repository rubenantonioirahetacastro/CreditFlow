namespace CreditFlow.API.Features.Roles.Shared.Permissions;

/// <summary>Permiso sobre una opción del menú de la Web (clave definida por la Web, p. ej. «mantenimientos.empleados»).</summary>
public class RolePermissionDto
{
    public string Clave { get; set; } = string.Empty;

    public bool Ver { get; set; }

    public bool Crear { get; set; }

    public bool Editar { get; set; }

    public bool Eliminar { get; set; }
}
