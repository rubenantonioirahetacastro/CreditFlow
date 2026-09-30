using ClosedXML.Excel;
using CreditFlow.Web.Features.Simulator.Models;

namespace CreditFlow.Web.Features.Simulator.Services;

public interface ICronogramaExcelExporter
{
    byte[] Exportar(SimularCalendarioResponse resultado, string? periodicidad);
}

public sealed class CronogramaExcelExporter : ICronogramaExcelExporter
{
    private const string FormatoMoneda = "$#,##0.00";
    private const string FormatoFecha = "dd/mm/yyyy";

    private static readonly string[] Encabezados =
        ["Cuota", "Vencimiento", "Capital", "Interés", "IVA", "Gastos", "Total", "Saldo"];

    public byte[] Exportar(SimularCalendarioResponse resultado, string? periodicidad)
    {
        using var libro = new XLWorkbook();
        var hoja = libro.Worksheets.Add("Cronograma");

        hoja.Cell(1, 1).Value = "Cronograma de cuotas";
        hoja.Cell(1, 1).Style.Font.SetBold().Font.SetFontSize(14);

        var fila = 3;
        EscribirResumen(hoja, ref fila, "Monto solicitado", resultado.MontoSolicitado, FormatoMoneda);
        EscribirResumen(hoja, ref fila, "Nro. de cuotas", resultado.Plazo, "0");
        if (!string.IsNullOrWhiteSpace(periodicidad))
            EscribirResumen(hoja, ref fila, "Periodicidad", periodicidad, null);
        EscribirResumen(hoja, ref fila, "Cuota", resultado.CuotaFija, FormatoMoneda);
        EscribirResumen(hoja, ref fila, "Tasa mensual", resultado.TasaNominalMensual / 100m, "0.00%");
        EscribirResumen(hoja, ref fila, "TEA real", resultado.TeaReal, "0.00%");
        EscribirResumen(hoja, ref fila, "Total a pagar", resultado.TotalPagado, FormatoMoneda);
        EscribirResumen(hoja, ref fila, "Costo del crédito", resultado.CostoTotalCredito, FormatoMoneda);
        if (!string.IsNullOrWhiteSpace(resultado.LineaUsada))
            EscribirResumen(hoja, ref fila, "Línea", resultado.LineaUsada, null);

        fila++;
        var filaEncabezado = fila;
        for (var i = 0; i < Encabezados.Length; i++)
            hoja.Cell(filaEncabezado, i + 1).Value = Encabezados[i];

        var encabezado = hoja.Range(filaEncabezado, 1, filaEncabezado, Encabezados.Length);
        encabezado.Style.Font.SetBold()
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
            .Border.SetBottomBorder(XLBorderStyleValues.Thin);

        foreach (var cuota in resultado.Cronograma)
        {
            fila++;
            hoja.Cell(fila, 1).Value = cuota.NroCuota;
            hoja.Cell(fila, 2).Value = cuota.FechaVencimiento;
            hoja.Cell(fila, 3).Value = cuota.Capital;
            hoja.Cell(fila, 4).Value = cuota.Interes;
            hoja.Cell(fila, 5).Value = cuota.Iva;
            hoja.Cell(fila, 6).Value = cuota.Gasto;
            hoja.Cell(fila, 7).Value = cuota.TotalCuota;
            hoja.Cell(fila, 8).Value = cuota.SaldoDespues;
        }

        fila++;
        hoja.Cell(fila, 1).Value = "Total";
        hoja.Cell(fila, 3).Value = resultado.TotalCapital;
        hoja.Cell(fila, 4).Value = resultado.TotalInteres;
        hoja.Cell(fila, 5).Value = resultado.TotalIva;
        hoja.Cell(fila, 6).Value = resultado.TotalGasto;
        hoja.Cell(fila, 7).Value = resultado.TotalPagado;
        hoja.Range(fila, 1, fila, Encabezados.Length).Style.Font.SetBold()
            .Border.SetTopBorder(XLBorderStyleValues.Thin);

        var primeraFilaDatos = filaEncabezado + 1;
        hoja.Range(primeraFilaDatos, 1, fila, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        hoja.Range(primeraFilaDatos, 2, fila, 2).Style.NumberFormat.Format = FormatoFecha;
        hoja.Range(primeraFilaDatos, 2, fila, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        hoja.Range(primeraFilaDatos, 3, fila, Encabezados.Length).Style.NumberFormat.Format = FormatoMoneda;

        hoja.SheetView.FreezeRows(filaEncabezado);
        hoja.Columns(1, Encabezados.Length).AdjustToContents();

        using var stream = new MemoryStream();
        libro.SaveAs(stream);
        return stream.ToArray();
    }

    private static void EscribirResumen(IXLWorksheet hoja, ref int fila, string etiqueta, XLCellValue valor, string? formato)
    {
        hoja.Cell(fila, 1).Value = etiqueta;
        hoja.Cell(fila, 1).Style.Font.SetBold();
        var celda = hoja.Cell(fila, 2);
        celda.Value = valor;
        celda.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
        if (formato is not null)
            celda.Style.NumberFormat.Format = formato;
        fila++;
    }
}
