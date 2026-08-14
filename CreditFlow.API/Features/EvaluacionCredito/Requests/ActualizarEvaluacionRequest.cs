namespace CreditFlow.API.Features.EvaluacionCredito.Requests;

/// <summary>
/// Actualiza el estado (Creditos.NEstado) de un crédito puntual desde la pantalla
/// de Evaluación. NEstado debe ser uno de los valores del catálogo de código 123.
/// </summary>
public class ActualizarEvaluacionRequest
{
    public int NCodAge { get; set; }

    public int NCodCred { get; set; }

    public int NEstado { get; set; }
}
