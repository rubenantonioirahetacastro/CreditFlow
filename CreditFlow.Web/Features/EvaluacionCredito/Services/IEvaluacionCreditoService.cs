using CreditFlow.Web.Features.EvaluacionCredito.Models;

namespace CreditFlow.Web.Features.EvaluacionCredito.Services;

public interface IEvaluacionCreditoService
{
    /// <summary>Actualiza Creditos.NEstado con el valor elegido en el Select de evaluación (catálogo 116).</summary>
    Task<(bool Exito, string? Mensaje)> ActualizarEvaluacionAsync(int nCodAge, int nCodCred, int nEstado);

    /// <summary>Datos del crédito (monto, estado actual) para precargar la pantalla de Evaluación.</summary>
    Task<DatosCreditoGestionDto?> ObtenerDatosCreditoAsync(int nCodAge, int nCodCred);

    /// <summary>Expediente completo (cliente, negocio, garantía, fiador, verificaciones) para la pantalla de Evaluación.</summary>
    Task<EvaluacionCreditoDetalleDto?> ObtenerDetalleCompletoAsync(int nCodAge, int nCodCred);

    Task<string?> ObtenerFotoDataUrlAsync(string tipo, int idFoto);
}
