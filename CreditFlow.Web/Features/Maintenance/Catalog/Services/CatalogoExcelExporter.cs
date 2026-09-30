using ClosedXML.Excel;
using CreditFlow.Web.Features.Maintenance.Catalog.Models;

namespace CreditFlow.Web.Features.Maintenance.Catalog.Services;

/// <summary>Exporta los valores de un catálogo a .xlsx (encabezado con el catálogo + tabla de valores).</summary>
public static class CatalogoExcelExporter
{
    public static byte[] Exportar(CatalogoGrupo grupo)
    {
        using var libro = new XLWorkbook();
        var hoja = libro.Worksheets.Add($"Catálogo {grupo.Codigo}");

        hoja.Cell(1, 1).Value = $"{grupo.Nombre} ({grupo.Codigo})";
        hoja.Cell(1, 1).Style.Font.SetBold().Font.SetFontSize(14);
        hoja.Cell(2, 1).Value = $"Clave {grupo.Clave} · {grupo.Activos} activos de {grupo.Valores.Count}";

        string[] encabezados = ["Valor", "Nombre", "Tipo", "Estado"];
        const int filaEncabezado = 4;
        for (var i = 0; i < encabezados.Length; i++)
            hoja.Cell(filaEncabezado, i + 1).Value = encabezados[i];
        hoja.Range(filaEncabezado, 1, filaEncabezado, encabezados.Length).Style.Font.SetBold()
            .Border.SetBottomBorder(XLBorderStyleValues.Thin);

        var fila = filaEncabezado;
        foreach (var valor in grupo.Valores)
        {
            fila++;
            hoja.Cell(fila, 1).Value = valor.NValor;
            hoja.Cell(fila, 2).Value = valor.CNomCod;
            hoja.Cell(fila, 3).Value = CatalogoConvenciones.EsSistema(valor) ? "Sistema" : "General";
            hoja.Cell(fila, 4).Value = CatalogoConvenciones.EsActivo(valor) ? "Activo" : "Inactivo";
        }

        hoja.SheetView.FreezeRows(filaEncabezado);
        hoja.Columns(1, encabezados.Length).AdjustToContents();

        using var stream = new MemoryStream();
        libro.SaveAs(stream);
        return stream.ToArray();
    }
}
