using CreditFlow.Web.Core.Http;
using CreditFlow.Web.Shared.Catalog.Models;

namespace CreditFlow.Web.Shared.Catalog.Services;

public class ObtenerCatalogoCodigosApi : ObtenerCatalogoCodigos
{
    private readonly IApiClient _apiClient;

    public ObtenerCatalogoCodigosApi(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<CatalogoCodigoDto>> ObtenerCatalogoCodigo(int nCodigo)
    {
        var result = await _apiClient.GetAsync<List<CatalogoCodigoDto>>(
            $"api/CatalogoCodigo/{nCodigo}",
            "No se pudo cargar el catálogo solicitado.");
        return result.Data ?? [];
    }
}
