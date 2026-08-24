namespace CreditFlow.Web.Shared.CatalogoCodigos.Models;

public class CatalogoCodigoDto
{
    public int NCodigo { get; set; }

    public int NValor { get; set; }

    public string CNomCod { get; set; } = string.Empty;

    public int? NEstados { get; set; }

    public int? NTipoCodigo { get; set; }
}
