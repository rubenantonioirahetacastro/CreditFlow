namespace CreditFlow.API.Domain.Entities;

/// <summary>
/// Permiso de un rol sobre una opción del menú de la Web. La opción se identifica por su clave estable
/// (p. ej. «mantenimientos.empleados»); el árbol de menús lo define la Web.
/// </summary>
public partial class RolMenuPermiso
{
    public int IdRolMenuPermiso { get; set; }

    public int IdRol { get; set; }

    public string CClaveMenu { get; set; } = null!;

    public bool BVer { get; set; }

    public bool BCrear { get; set; }

    public bool BEditar { get; set; }

    public bool BEliminar { get; set; }

    public DateTime DFechaModificacion { get; set; }

    public virtual Role IdRolNavigation { get; set; } = null!;
}
