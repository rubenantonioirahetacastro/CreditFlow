namespace CreditFlow.Web.Features.Maintenance.Role.Validation;

/// <summary>Errores por campo para la validación en vivo del formulario de rol.</summary>
public sealed record RoleFormErrors(string? Nombre, string? Duplicado)
{
    public bool HayErrores => Nombre is not null || Duplicado is not null;
}

public static class RoleValidator
{
    /// <summary>Nombre obligatorio y sin repetir entre los demás roles (sin distinguir mayúsculas).</summary>
    public static RoleFormErrors Validate(string nombre, IEnumerable<string> nombresExistentes)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return new RoleFormErrors("El nombre del rol es obligatorio.", null);

        var repetido = nombresExistentes.Any(n => string.Equals(n.Trim(), nombre.Trim(), StringComparison.OrdinalIgnoreCase));
        return new RoleFormErrors(null, repetido ? $"Ya existe un rol llamado «{nombre.Trim()}»." : null);
    }
}
