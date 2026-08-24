using System.Text.Json.Serialization;

namespace CreditFlow.Web.Features.EvaluacionCredito.Models;

public class DatosCreditoGestionDto
{
    [JsonPropertyName("nCodAge")]
    public int NCodAge { get; set; }

    [JsonPropertyName("nCodCred")]
    public int NCodCred { get; set; }

    [JsonPropertyName("nPrestamo")]
    public decimal NPrestamo { get; set; }

    [JsonPropertyName("nEstado")]
    public int NEstado { get; set; }
}
