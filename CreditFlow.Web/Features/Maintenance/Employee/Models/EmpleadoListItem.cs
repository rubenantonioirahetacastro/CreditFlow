using CreditFlow.Web.Core.Utils.Format;

namespace CreditFlow.Web.Features.Maintenance.Employee.Models;

/// <summary>
/// Fila de la tabla de empleados. Expone como propiedades los textos visibles para que
/// ordenar, filtrar y exportar a Excel usen exactamente lo que se ve.
/// </summary>
public sealed class EmpleadoListItem
{
    private static readonly string[] ColoresAvatar =
    [
        "var(--cds-primary)",
        "var(--cds-primary-accent-light)",
        "var(--cds-primary-light)",
        "var(--cds-primary-accent-dark)",
        "var(--cds-primary-dark)"
    ];

    public EmpleadoListItem(EmpleadoDto empleado, string? agencia)
    {
        Empleado = empleado;
        NombreVisible = NameFormatter.ToDisplayName(
            $"{empleado.Nombres} {empleado.PrimerApellido} {empleado.SegundoApellido}");
        UsuarioVisible = string.IsNullOrWhiteSpace(empleado.CodigoUsuario) ? "Sin usuario" : empleado.CodigoUsuario;
        DocumentoVisible = string.IsNullOrWhiteSpace(empleado.Documento)
            ? "Sin documento"
            : DocumentFormatter.Format(empleado.Documento, DocumentType.Dui);
        AgenciaVisible = agencia ?? (empleado.CodAgencia > 0 ? $"Agencia {empleado.CodAgencia}" : "Sin agencia");
        RolVisible = string.IsNullOrWhiteSpace(empleado.Rol) ? "Sin rol" : empleado.Rol;
        CorreoVisible = string.IsNullOrWhiteSpace(empleado.Correo) ? "Sin correo" : empleado.Correo;
        TelefonoVisible = string.IsNullOrWhiteSpace(empleado.Telefono) ? "Sin teléfono" : PhoneFormatter.Format(empleado.Telefono);
        EstadoVisible = EmpleadoConvenciones.TextoEstado(empleado.Estado);
    }

    public EmpleadoDto Empleado { get; }

    public int IdEmpleado => Empleado.IdEmpleado;

    public bool Activo => EmpleadoConvenciones.EsActivo(Empleado);

    public bool TieneFoto => !string.IsNullOrEmpty(Empleado.FotoUrl);

    public string UsuarioVisible { get; }

    public string NombreVisible { get; }

    public string DocumentoVisible { get; }

    public string AgenciaVisible { get; }

    public string RolVisible { get; }

    public string CorreoVisible { get; }

    public string TelefonoVisible { get; }

    public string EstadoVisible { get; }

    public string Iniciales => ObtenerIniciales(Empleado.Nombres, Empleado.PrimerApellido);

    public string ColorAvatar => ObtenerColorAvatar(Empleado.Nombres, Empleado.PrimerApellido);

    public static string ObtenerIniciales(string? nombres, string? primerApellido)
    {
        var inicialNombre = string.IsNullOrWhiteSpace(nombres) ? "" : nombres.Trim()[..1];
        var inicialApellido = string.IsNullOrWhiteSpace(primerApellido) ? "" : primerApellido.Trim()[..1];
        return (inicialNombre + inicialApellido).ToUpperInvariant();
    }

    /// <summary>Color estable por persona (no usa GetHashCode, que cambia en cada ejecución).</summary>
    public static string ObtenerColorAvatar(string? nombres, string? primerApellido)
    {
        var clave = $"{nombres}{primerApellido}".Trim().ToUpperInvariant();
        var suma = clave.Aggregate(0, (acumulado, letra) => acumulado + letra);
        return ColoresAvatar[suma % ColoresAvatar.Length];
    }
}
