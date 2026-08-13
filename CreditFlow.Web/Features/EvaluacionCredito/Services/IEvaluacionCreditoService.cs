namespace CreditFlow.Web.Features.EvaluacionCredito.Services;

public interface IEvaluacionCreditoService
{
    /// <summary>Actualiza Creditos.NEstado con el valor elegido en el Select de evaluación (catálogo 123).</summary>
    Task<(bool Exito, string? Mensaje)> ActualizarEvaluacionAsync(int nCodAge, int nCodCred, int nEstado);
}
