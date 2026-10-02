namespace CreditFlow.Web.Features.Maintenance.Employee.Models;

/// <summary>
/// Valores numéricos de Empleado que la pantalla interpreta.
/// PENDIENTE DE CONFIRMAR con backend: la API guarda nSexo como número sin catálogo publicado.
/// Se asume 1 = Masculino y 2 = Femenino. nEstado 1 = Activo, 0 = Inactivo (igual que la API).
/// Si cambian, se ajustan solo aquí.
/// </summary>
public static class EmpleadoConvenciones
{
    public const int EstadoActivo = 1;
    public const int EstadoInactivo = 0;

    public const int SexoMasculino = 1;
    public const int SexoFemenino = 2;

    public static bool EsActivo(EmpleadoDto empleado) => empleado.Estado == EstadoActivo;

    public static string NombreEstado(int estado) => estado == EstadoActivo ? "Activo" : "Inactivo";

    public static string NombreSexo(int sexo) => sexo switch
    {
        SexoMasculino => "Masculino",
        SexoFemenino => "Femenino",
        _ => "Sin especificar"
    };
}
