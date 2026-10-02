using CreditFlow.Web.Core.Utils.Format;

namespace CreditFlow.Web.Features.Maintenance.Employee.Models;

/// <summary>
/// Fila del listado de empleados: el registro de la API más los textos visibles ya resueltos,
/// para que ordenar, filtrar y exportar usen exactamente lo que se ve.
/// </summary>
public sealed class EmpleadoListItem
{
    public required EmpleadoDto Empleado { get; init; }

    public int IdEmpleado => Empleado.IdEmpleado;

    public string CodigoUsuario => string.IsNullOrWhiteSpace(Empleado.CodigoUsuario) ? "Sin usuario" : Empleado.CodigoUsuario;

    public string NombreCompleto { get; init; } = string.Empty;

    public string Iniciales { get; init; } = string.Empty;

    public string Documento => Empleado.Documento;

    public string AgenciaNombre { get; init; } = string.Empty;

    public string RolNombre { get; init; } = string.Empty;

    public string Correo => string.IsNullOrWhiteSpace(Empleado.Correo) ? "Sin correo" : Empleado.Correo;

    public string Telefono => string.IsNullOrWhiteSpace(Empleado.Telefono) ? "Sin teléfono" : Empleado.Telefono;

    public string SexoTexto => EmpleadoConvenciones.NombreSexo(Empleado.Sexo);

    public bool Activo => EmpleadoConvenciones.EsActivo(Empleado);

    public string EstadoTexto => EmpleadoConvenciones.NombreEstado(Empleado.Estado);

    public bool TieneFoto => !string.IsNullOrWhiteSpace(Empleado.FotoUrl);

    public static EmpleadoListItem Crear(EmpleadoDto empleado, IReadOnlyDictionary<int, string> agencias)
    {
        var nombre = NameFormatter.ToDisplayName(
            $"{empleado.Nombres} {empleado.PrimerApellido} {empleado.SegundoApellido}");

        return new EmpleadoListItem
        {
            Empleado = empleado,
            NombreCompleto = nombre,
            Iniciales = ObtenerIniciales(empleado),
            AgenciaNombre = agencias.TryGetValue(empleado.CodAgencia, out var agencia)
                ? agencia
                : empleado.CodAgencia > 0 ? $"Agencia {empleado.CodAgencia}" : "Sin agencia",
            RolNombre = string.IsNullOrWhiteSpace(empleado.Rol) ? "Sin rol" : empleado.Rol
        };
    }

    public bool Coincide(string texto) =>
        NombreCompleto.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
        CodigoUsuario.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
        Documento.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
        Correo.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
        AgenciaNombre.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
        RolNombre.Contains(texto, StringComparison.OrdinalIgnoreCase);

    private static string ObtenerIniciales(EmpleadoDto empleado)
    {
        var nombre = empleado.Nombres.Trim();
        var apellido = empleado.PrimerApellido.Trim();
        return $"{(nombre.Length > 0 ? nombre[0] : ' ')}{(apellido.Length > 0 ? apellido[0] : ' ')}"
            .Trim()
            .ToUpperInvariant();
    }
}
