using CreditFlow.Web.Core.Http;
using CreditFlow.Web.Shared.CatalogoCodigos.Models;

namespace CreditFlow.Web.Shared.CatalogoCodigos.Services;

public class ObtenerCatalogoCodigosApi : ObtenerCatalogoCodigos
{
    private readonly IApiClient _apiClient;

    public ObtenerCatalogoCodigosApi(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<CatalogoCodigoDto>> ObtenerCatalogoCodigo(int nCodigo)
    {
        var catalogo = await _apiClient.GetAsync<List<CatalogoCodigoDto>>($"api/CatalogoCodigo/{nCodigo}");
        return catalogo ?? new List<CatalogoCodigoDto>();
    }
}
