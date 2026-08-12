namespace CreditFlow.Web.Models.Mantenimientos;

// Refleja la entidad CatalogoCodigo de la API (NCodigo+NValor es la clave compuesta).
public class CatalogoCodigoDto
{
    public int NCodigo { get; set; }

    public int NValor { get; set; }

    public string CNomCod { get; set; } = string.Empty;

    public int? NEstados { get; set; }

    public int? NTipoCodigo { get; set; }
}
