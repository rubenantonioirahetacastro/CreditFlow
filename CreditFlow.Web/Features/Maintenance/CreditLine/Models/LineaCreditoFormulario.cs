using System.Globalization;

namespace CreditFlow.Web.Features.Maintenance.CreditLine.Models;

/// <summary>
/// Estado del formulario de crear/editar línea de crédito (panel lateral). Los números se editan como texto
/// para validar en vivo lo que el usuario escribe; <see cref="ADto"/> los convierte una vez validados.
/// </summary>
public sealed class LineaCreditoFormulario
{
    private static readonly CultureInfo Invariante = CultureInfo.InvariantCulture;

    public string Descripcion { get; set; } = string.Empty;

    public string Producto { get; set; } = string.Empty;

    public string SubProducto { get; set; } = string.Empty;

    public string TasaComision { get; set; } = "0";

    public string PlazoMinimo { get; set; } = string.Empty;

    public string PlazoMaximo { get; set; } = string.Empty;

    public string MontoMinimo { get; set; } = string.Empty;

    public string MontoMaximo { get; set; } = string.Empty;

    public string PrestamosMinimo { get; set; } = string.Empty;

    public string PrestamosMaximo { get; set; } = string.Empty;

    public bool AplicaRefinanciamiento { get; set; }

    public bool Activa { get; set; } = true;

    public static LineaCreditoFormulario Desde(LineaCreditoDto linea) => new()
    {
        Descripcion = linea.Descripcion,
        Producto = linea.Producto.ToString(Invariante),
        SubProducto = linea.SubProducto.ToString(Invariante),
        TasaComision = linea.TasaComision.ToString("0.##", Invariante),
        PlazoMinimo = linea.PlazoMinimo.ToString(Invariante),
        PlazoMaximo = linea.PlazoMaximo.ToString(Invariante),
        MontoMinimo = linea.MontoMinimo.ToString("0.##", Invariante),
        MontoMaximo = linea.MontoMaximo.ToString("0.##", Invariante),
        PrestamosMinimo = linea.NumeroPrestamosMinimo?.ToString(Invariante) ?? string.Empty,
        PrestamosMaximo = linea.NumeroPrestamosMaximo?.ToString(Invariante) ?? string.Empty,
        AplicaRefinanciamiento = linea.AplicaRefinanciamiento == true,
        Activa = linea.Activa
    };

    /// <summary>Convierte el formulario ya validado (ver CreditLineValidator).</summary>
    public LineaCreditoDto ADto(int nCodLinea) => new()
    {
        NCodLinea = nCodLinea,
        Descripcion = Descripcion.Trim(),
        Producto = Entero(Producto) ?? 0,
        SubProducto = Entero(SubProducto) ?? 0,
        TasaComision = Decimal(TasaComision) ?? 0,
        PlazoMinimo = Entero(PlazoMinimo) ?? 0,
        PlazoMaximo = Entero(PlazoMaximo) ?? 0,
        MontoMinimo = Decimal(MontoMinimo) ?? 0,
        MontoMaximo = Decimal(MontoMaximo) ?? 0,
        NumeroPrestamosMinimo = Entero(PrestamosMinimo),
        NumeroPrestamosMaximo = Entero(PrestamosMaximo),
        AplicaRefinanciamiento = AplicaRefinanciamiento,
        Activa = Activa
    };

    /// <summary>Entero sin signo; nulo si está vacío o no es un número.</summary>
    public static int? Entero(string texto) =>
        int.TryParse(texto.Trim(), NumberStyles.None, Invariante, out var valor) ? valor : null;

    /// <summary>Decimal con punto como separador; acepta comas de miles («5,000.50»). Nulo si está vacío o no es un número.</summary>
    public static decimal? Decimal(string texto) =>
        decimal.TryParse(texto.Trim().Replace(",", string.Empty), NumberStyles.AllowDecimalPoint, Invariante, out var valor)
            ? valor
            : null;
}
