namespace CreditFlow.Web.Features.Maintenance.Agency.Models;

/// <summary>Fila del listado de agencias con los textos visibles resueltos (ordenar, filtrar y exportar usan lo que se ve).</summary>
public sealed class AgenciaListItem
{
    public required AgenciaDto Agencia { get; init; }

    public int Codigo => Agencia.NCodAge;

    public string Nombre => Agencia.Nombre;

    public string Direccion => string.IsNullOrWhiteSpace(Agencia.Direccion) ? "Sin dirección" : Agencia.Direccion;

    public string Telefono => string.IsNullOrWhiteSpace(Agencia.Telefono) ? "Sin teléfono" : Agencia.Telefono;

    public string Correo => string.IsNullOrWhiteSpace(Agencia.CorreoElectronico) ? "Sin correo" : Agencia.CorreoElectronico;

    public bool TieneTelefono => !string.IsNullOrWhiteSpace(Agencia.Telefono);

    public bool TieneCorreo => !string.IsNullOrWhiteSpace(Agencia.CorreoElectronico);

    /// <summary>Le falta dirección, teléfono o correo.</summary>
    public bool DatosIncompletos => string.IsNullOrWhiteSpace(Agencia.Direccion) || !TieneTelefono || !TieneCorreo;

    public bool Coincide(string texto) =>
        Nombre.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
        Direccion.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
        Correo.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
        Telefono.Contains(texto) ||
        Codigo.ToString().Contains(texto);
}
