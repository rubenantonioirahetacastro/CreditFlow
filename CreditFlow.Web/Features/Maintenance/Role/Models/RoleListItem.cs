namespace CreditFlow.Web.Features.Maintenance.Role.Models;

/// <summary>Fila del listado de roles con los textos visibles resueltos (ordenar, filtrar y exportar usan lo que se ve).</summary>
public sealed class RoleListItem
{
    public required RoleDto Rol { get; init; }

    public int IdRol => Rol.IdRol;

    public string Nombre => Rol.Nombre;

    public string Descripcion => string.IsNullOrWhiteSpace(Rol.Descripcion) ? "Sin descripción" : Rol.Descripcion;

    public bool Activo => Rol.Activo;

    public string EstadoTexto => Rol.Activo ? "Activo" : "Inactivo";

    public bool Coincide(string texto) =>
        Nombre.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
        Descripcion.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
        IdRol.ToString().Contains(texto);
}
