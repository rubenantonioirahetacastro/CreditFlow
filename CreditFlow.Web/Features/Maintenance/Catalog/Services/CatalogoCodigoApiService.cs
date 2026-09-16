using CreditFlow.Web.Core.Http;
using CreditFlow.Web.Shared.Catalog.Models;

namespace CreditFlow.Web.Features.Maintenance.Catalog.Services;

public sealed class CatalogoCodigoApiService(IApiClient apiClient) : ICatalogoCodigoService
{
    private const string BaseUrl = "api/CatalogoCodigo";

    public Task<ApiResult<List<CatalogoCodigoDto>>> ObtenerTodosAsync() =>
        apiClient.GetAsync<List<CatalogoCodigoDto>>(
            BaseUrl,
            "No se pudieron cargar los códigos de catálogo.");

    public async Task<(bool Exito, string? Mensaje)> CrearAsync(CatalogoCodigoDto catalogo)
    {
        var result = await apiClient.PostAsync(BaseUrl, catalogo, "No se pudo crear el código de catálogo.");
        return (result.IsSuccess, result.Message);
    }

    public async Task<(bool Exito, string? Mensaje)> ActualizarAsync(CatalogoCodigoDto catalogo)
    {
        var result = await apiClient.PutAsync(BaseUrl, catalogo, "No se pudo actualizar el código de catálogo.");
        return (result.IsSuccess, result.Message);
    }

    public async Task<(bool Exito, string? Mensaje)> EliminarAsync(int nCodigo, int nValor)
    {
        var result = await apiClient.DeleteAsync(
            $"{BaseUrl}/{nCodigo}/{nValor}",
            "No se pudo eliminar el código de catálogo.");
        return (result.IsSuccess, result.Message);
    }
}
