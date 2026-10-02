namespace CreditFlow.Web.Features.Maintenance.Employee.Components;

/// <summary>Color de fondo del avatar con iniciales, estable para cada empleado (no cambia entre sesiones).</summary>
public static class EmployeeAvatarPalette
{
    private static readonly string[] Colores =
    [
        "var(--cds-primary)",
        "var(--cds-primary-accent-light)",
        "var(--cds-primary-dark)",
        "var(--cds-primary-darker)"
    ];

    public static string Resolve(int idEmpleado, string iniciales)
    {
        // Suma simple de caracteres: string.GetHashCode cambia en cada arranque del servidor.
        var clave = idEmpleado + iniciales.Sum(c => c);
        return Colores[Math.Abs(clave) % Colores.Length];
    }
}
