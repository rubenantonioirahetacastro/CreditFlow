using System.Text.Json.Serialization;

namespace CreditFlow.Web.Features.Verification.Models;

public class VerificacionListItem
{
    [JsonPropertyName("nCodCred")]
    public int NCodCred { get; set; }

    [JsonPropertyName("nCodAge")]
    public int NCodAge { get; set; }

    [JsonPropertyName("agencia")]
    public string? Agencia { get; set; }

    [JsonPropertyName("nombreCliente")]
    public string? NombreCliente { get; set; }

    [JsonPropertyName("idPersona")]
    public int? IdPersona { get; set; }

    [JsonPropertyName("fotoUrl")]
    public string? FotoUrl { get; set; }

    [JsonPropertyName("usuarioGestion")]
    public string? UsuarioGestion { get; set; }

    [JsonPropertyName("montoSolicitado")]
    public decimal MontoSolicitado { get; set; }

    [JsonPropertyName("dFecVig")]
    public DateTime DFecVig { get; set; }

    [JsonPropertyName("conRepretamo")]
    public bool ConRepretamo { get; set; }

    [JsonPropertyName("nEstado")]
    public int NEstado { get; set; }

    [JsonPropertyName("estado")]
    public string? Estado { get; set; }

    [JsonPropertyName("nSubProd")]
    public int NSubProd { get; set; }

    [JsonPropertyName("subproducto")]
    public string? SubProducto { get; set; }
}
