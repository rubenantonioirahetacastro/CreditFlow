using CreditFlow.Web.Core.Http;

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
}
