using CreditFlow.Web.Core.Http;
using CreditFlow.Web.Features.BandejaVerificacion.Models;

namespace CreditFlow.Web.Features.BandejaVerificacion.Services;

public class VerificacionApiService : IVerificacionService
{
    private readonly IApiClient _apiClient;

    public VerificacionApiService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<VerificacionListItem>> ObtenerBandejaAsync(int? nCodAge = null)
    {
        var url = "api/BandejaVerificacion";
        if (nCodAge.HasValue)
            url += $"?nCodAge={nCodAge.Value}";

        var result = await _apiClient.GetAsync<List<VerificacionListItem>>(
            url,
            "No se pudo cargar la bandeja de verificación.");
        return result.Data ?? [];
    }

    public async Task<string?> ObtenerFotoDataUrlAsync(int idPersona)
    {
        var result = await _apiClient.GetImageDataUrlAsync(
            $"api/BuscarSolicitudCredito/{idPersona}/foto");
        return result.Data;
    }
}
