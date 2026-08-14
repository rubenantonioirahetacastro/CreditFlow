using CreditFlow.Web.Core.Http;
using CreditFlow.Web.Shared.CatalogoCodigos.Models;

namespace CreditFlow.Web.Features.Mantenimientos.CatalogosCodigos.Services;

public class CatalogoCodigoApiService : ICatalogoCodigoService
{
    private readonly IApiClient _apiClient;

    public CatalogoCodigoApiService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<CatalogoCodigoDto>> ObtenerTodosAsync()
    {
        var catalogos = await _apiClient.GetAsync<List<CatalogoCodigoDto>>("api/CatalogoCodigo");
        return catalogos ?? new List<CatalogoCodigoDto>();
    }

    public Task<(bool Exito, string? Mensaje)> CrearAsync(CatalogoCodigoDto catalogo)
        => _apiClient.PostAsync("api/CatalogoCodigo", catalogo);

    public Task<(bool Exito, string? Mensaje)> ActualizarAsync(CatalogoCodigoDto catalogo)
        => _apiClient.PutAsync("api/CatalogoCodigo", catalogo);

    public Task<(bool Exito, string? Mensaje)> EliminarAsync(int nCodigo, int nValor)
        => _apiClient.DeleteAsync($"api/CatalogoCodigo/{nCodigo}/{nValor}");
}
