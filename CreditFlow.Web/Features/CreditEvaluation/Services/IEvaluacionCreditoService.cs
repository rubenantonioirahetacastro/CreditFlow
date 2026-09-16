using CreditFlow.Web.Core.Http;
using CreditFlow.Web.Features.CreditEvaluation.Models;

namespace CreditFlow.Web.Features.CreditEvaluation.Services;

public interface IEvaluacionCreditoService
{
    Task<(bool Exito, string? Mensaje)> ActualizarEvaluacionAsync(int nCodAge, int nCodCred, int nEstado);

    Task<DatosCreditoGestionDto?> ObtenerDatosCreditoAsync(int nCodAge, int nCodCred);

    Task<ApiResult<EvaluacionCreditoDetalleDto>> ObtenerDetalleCompletoAsync(int nCodAge, int nCodCred);

    Task<string?> ObtenerFotoDataUrlAsync(string tipo, int idFoto);
}
