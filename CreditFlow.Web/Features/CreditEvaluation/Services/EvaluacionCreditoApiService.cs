using CreditFlow.Web.Core.Http;
using CreditFlow.Web.Features.CreditEvaluation.Models;

namespace CreditFlow.Web.Features.CreditEvaluation.Services;

public class EvaluacionCreditoApiService : IEvaluacionCreditoService
{
    private readonly IApiClient _apiClient;

    public EvaluacionCreditoApiService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<(bool Exito, string? Mensaje)> ActualizarEvaluacionAsync(int nCodAge, int nCodCred, int nEstado)
    {
        var result = await _apiClient.PutAsync(
            "api/Credito/actualizar-evaluacion",
            new { nCodAge, nCodCred, nEstado },
            "No se pudo actualizar la evaluación.");
        return (result.IsSuccess, result.Message);
    }

    public async Task<DatosCreditoGestionDto?> ObtenerDatosCreditoAsync(int nCodAge, int nCodCred)
    {
        var result = await _apiClient.GetAsync<DatosCreditoGestionDto>(
            $"api/DatosCreditoGestion/{nCodAge}/{nCodCred}",
            "No se pudieron cargar los datos del crédito.");
        return result.Data;
    }

    public Task<ApiResult<EvaluacionCreditoDetalleDto>> ObtenerDetalleCompletoAsync(int nCodAge, int nCodCred) =>
        _apiClient.GetAsync<EvaluacionCreditoDetalleDto>(
            $"api/EvaluacionCreditoDetalle/{nCodAge}/{nCodCred}",
            "No se pudo cargar el expediente del crédito.");

    public async Task<string?> ObtenerFotoDataUrlAsync(string tipo, int idFoto)
    {
        var result = await _apiClient.GetImageDataUrlAsync(
            $"api/BuscarSolicitudCredito/foto/{tipo}/{idFoto}");
        return result.Data;
    }
}
