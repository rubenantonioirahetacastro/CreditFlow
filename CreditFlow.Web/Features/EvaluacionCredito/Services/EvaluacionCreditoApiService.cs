using CreditFlow.Web.Core.Http;
using CreditFlow.Web.Features.EvaluacionCredito.Models;

namespace CreditFlow.Web.Features.EvaluacionCredito.Services;

public class EvaluacionCreditoApiService : IEvaluacionCreditoService
{
    private readonly IApiClient _apiClient;

    public EvaluacionCreditoApiService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<(bool Exito, string? Mensaje)> ActualizarEvaluacionAsync(int nCodAge, int nCodCred, int nEstado)
    {
        return _apiClient.PutAsync("api/Credito/actualizar-evaluacion", new { nCodAge, nCodCred, nEstado });
    }

    public Task<DatosCreditoGestionDto?> ObtenerDatosCreditoAsync(int nCodAge, int nCodCred)
    {
        return _apiClient.GetAsync<DatosCreditoGestionDto>($"api/DatosCreditoGestion/{nCodAge}/{nCodCred}");
    }
}
