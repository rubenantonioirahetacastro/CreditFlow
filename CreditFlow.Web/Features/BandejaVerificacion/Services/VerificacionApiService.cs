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
        var url = "api/Credito/listaCreditos-cpc";
        if (nCodAge.HasValue)
            url += $"?ncodage={nCodAge.Value}";

        var creditos = await _apiClient.GetAsync<List<VerificacionListItem>>(url);
        return creditos ?? new List<VerificacionListItem>();
    }
}
