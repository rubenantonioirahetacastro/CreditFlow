using CreditFlow.Web.Shared.Catalog.Models;

namespace CreditFlow.Web.Features.Maintenance.Catalog.Models;

/// <summary>Un catálogo (maestro) con sus valores (detalle), agrupados por nCodigo.</summary>
public sealed class CatalogoGrupo
{
    public int Codigo { get; init; }

    /// <summary>Fila encabezado (nValor == nCodigo). Puede faltar en datos antiguos.</summary>
    public CatalogoCodigoDto? Encabezado { get; init; }

    public IReadOnlyList<CatalogoCodigoDto> Valores { get; init; } = [];

    public string Clave => Encabezado?.CNomCod ?? $"CATALOGO_{Codigo}";

    public string Nombre => CatalogoConvenciones.NombreLegible(Clave);

    public int Activos => Valores.Count(CatalogoConvenciones.EsActivo);

    public int Inactivos => Valores.Count - Activos;

    public int SiguienteValor => Valores.Count == 0 ? Codigo + 1 : Valores.Max(v => v.NValor) + 1;

    public static List<CatalogoGrupo> Agrupar(IEnumerable<CatalogoCodigoDto> filas) =>
        filas
            .GroupBy(f => f.NCodigo)
            .Select(g => new CatalogoGrupo
            {
                Codigo = g.Key,
                Encabezado = g.FirstOrDefault(CatalogoConvenciones.EsEncabezado),
                Valores = g.Where(f => !CatalogoConvenciones.EsEncabezado(f)).OrderBy(f => f.NValor).ToList()
            })
            .OrderBy(g => g.Codigo)
            .ToList();
}
