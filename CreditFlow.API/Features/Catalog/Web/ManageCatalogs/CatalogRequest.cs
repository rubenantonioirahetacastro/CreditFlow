using System.ComponentModel.DataAnnotations;

namespace CreditFlow.API.Features.Catalog.Web.ManageCatalogs;

public sealed class CatalogRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "El código de catálogo debe ser mayor que cero.")]
    public int NCodigo { get; set; }
    public int NValor { get; set; }
    [Required(ErrorMessage = "El nombre del valor de catálogo es requerido.")]
    public string CNomCod { get; set; } = null!;
    public int? NEstados { get; set; }
    public int? NTipoCodigo { get; set; }
}
