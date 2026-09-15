using System.ComponentModel.DataAnnotations;

namespace CreditFlow.API.Features.Credit.Web.UpdateCreditDecision;

/// <summary>
/// Actualiza el estado (Creditos.NEstado) de un crédito puntual desde la pantalla
/// de Evaluación. NEstado debe ser uno de los valores permitidos del catálogo 116.
/// </summary>
public sealed class UpdateCreditDecisionRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "La agencia es obligatoria.")]
    public int NCodAge { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "El crédito es obligatorio.")]
    public int NCodCred { get; set; }

    public int NEstado { get; set; }
}
