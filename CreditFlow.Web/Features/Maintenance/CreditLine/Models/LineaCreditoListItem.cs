using System.Globalization;

namespace CreditFlow.Web.Features.Maintenance.CreditLine.Models;

/// <summary>Fila del listado de líneas de crédito con los textos visibles resueltos (ordenar, filtrar y exportar usan lo que se ve).</summary>
public sealed class LineaCreditoListItem
{
    private static readonly CultureInfo Cultura = CultureInfo.GetCultureInfo("es-SV");

    public required LineaCreditoDto Linea { get; init; }

    public int Codigo => Linea.NCodLinea;

    public string Descripcion => Linea.Descripcion;

    public int Producto => Linea.Producto;

    public int SubProducto => Linea.SubProducto;

    public string Tasa => $"{Linea.TasaComision.ToString("N2", Cultura)} %";

    public string Plazo => $"{Linea.PlazoMinimo} – {Linea.PlazoMaximo}";

    public string Monto => $"{Linea.MontoMinimo.ToString("C2", Cultura)} – {Linea.MontoMaximo.ToString("C2", Cultura)}";

    public string Prestamos => (Linea.NumeroPrestamosMinimo, Linea.NumeroPrestamosMaximo) switch
    {
        (null, null) => "Sin límite",
        ({ } minimo, null) => $"Desde {minimo}",
        (null, { } maximo) => $"Hasta {maximo}",
        ({ } minimo, { } maximo) => $"{minimo} – {maximo}"
    };

    public bool Refinancia => Linea.AplicaRefinanciamiento == true;

    public string RefinanciaTexto => Refinancia ? "Sí" : "No";

    public bool Activa => Linea.Activa;

    public string EstadoTexto => Linea.Activa ? "Activa" : "Inactiva";

    public string Usuario => string.IsNullOrWhiteSpace(Linea.Usuario) ? "Sin usuario" : Linea.Usuario;

    public bool Coincide(string texto) =>
        Descripcion.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
        Usuario.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
        Codigo.ToString().Contains(texto) ||
        Producto.ToString() == texto ||
        SubProducto.ToString() == texto;
}
