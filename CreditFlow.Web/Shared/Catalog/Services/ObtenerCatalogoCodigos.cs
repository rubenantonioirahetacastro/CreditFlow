using CreditFlow.Web.Shared.Catalog.Models;

namespace CreditFlow.Web.Shared.Catalog.Services;

public interface ObtenerCatalogoCodigos
{
    Task<List<CatalogoCodigoDto>> ObtenerCatalogoCodigo(int nCodigo);
}
