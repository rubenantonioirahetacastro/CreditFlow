using CreditFlow.Web.Core.Http;
using CreditFlow.Web.Features.Verification.Models;

namespace CreditFlow.Web.Features.Verification.Services;

public class VerificacionApiService : IVerificacionService
{
    private readonly IApiClient _apiClient;

    public VerificacionApiService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<ApiResult<List<VerificacionListItem>>> ObtenerBandejaAsync(int? nCodAge = null)
    {
        var url = "api/BandejaVerificacion";
        if (nCodAge.HasValue)
            url += $"?nCodAge={nCodAge.Value}";

        return _apiClient.GetAsync<List<VerificacionListItem>>(
            url,
            "No se pudo cargar la bandeja de verificación.");
    }

    public async Task<string?> ObtenerFotoDataUrlAsync(int idPersona)
    {
        var result = await _apiClient.GetImageDataUrlAsync(
            $"api/BuscarSolicitudCredito/{idPersona}/foto");
        return result.Data;
    }
}
