using CreditFlow.Web.Shared.CatalogoCodigos.Models;

namespace CreditFlow.Web.Shared.CatalogoCodigos.Services;

public interface ObtenerCatalogoCodigos
{
    Task<List<CatalogoCodigoDto>> ObtenerCatalogoCodigo(int nCodigo);
}
