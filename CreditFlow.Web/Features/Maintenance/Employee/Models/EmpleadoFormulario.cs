using Microsoft.AspNetCore.Components.Forms;

namespace CreditFlow.Web.Features.Maintenance.Employee.Models;

/// <summary>Estado del formulario de crear/editar empleado (panel lateral).</summary>
public sealed class EmpleadoFormulario
{
    public string Documento { get; set; } = string.Empty;

    public string Nombres { get; set; } = string.Empty;

    public string PrimerApellido { get; set; } = string.Empty;

    public string SegundoApellido { get; set; } = string.Empty;

    public int Sexo { get; set; }

    public int CodAgencia { get; set; }

    public string Correo { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public int IdRol { get; set; }

    public bool Activo { get; set; } = true;

    public IBrowserFile? Foto { get; set; }

    public static EmpleadoFormulario Desde(EmpleadoDto empleado) => new()
    {
        Documento = empleado.Documento,
        Nombres = empleado.Nombres,
        PrimerApellido = empleado.PrimerApellido,
        SegundoApellido = empleado.SegundoApellido,
        Sexo = empleado.Sexo,
        CodAgencia = empleado.CodAgencia,
        Correo = empleado.Correo,
        Telefono = empleado.Telefono,
        IdRol = empleado.IdRol,
        Activo = EmpleadoConvenciones.EsActivo(empleado)
    };

    public CrearEmpleadoRequest ACrear() => new()
    {
        Documento = Documento.Trim(),
        Nombres = Nombres.Trim(),
        PrimerApellido = PrimerApellido.Trim(),
        SegundoApellido = SegundoApellido.Trim(),
        Sexo = Sexo,
        CodAgencia = CodAgencia,
        Correo = Correo.Trim(),
        Telefono = Telefono.Trim(),
        Password = Password,
        IdRol = IdRol,
        Foto = Foto
    };

    public ActualizarEmpleadoRequest AActualizar() => new()
    {
        Nombres = Nombres.Trim(),
        PrimerApellido = PrimerApellido.Trim(),
        SegundoApellido = SegundoApellido.Trim(),
        Sexo = Sexo,
        CodAgencia = CodAgencia,
        Correo = Correo.Trim(),
        Telefono = Telefono.Trim(),
        Estado = Activo ? EmpleadoConvenciones.EstadoActivo : EmpleadoConvenciones.EstadoInactivo,
        IdRol = IdRol,
        Foto = Foto
    };
}
