namespace CreditFlow.API.Features.Verification.Web.GetCreditRequests;

public sealed class BandejaVerificacionItemDto
{
    public int NCodCred { get; init; }
    public int NCodAge { get; init; }
    public string? Agencia { get; init; }
    public string? NombreCliente { get; init; }
    public int? IdPersona { get; init; }
    public string? FotoUrl { get; init; }
    public string? UsuarioGestion { get; init; }
    public decimal MontoSolicitado { get; init; }
    public DateTime DFecVig { get; init; }
    public bool ConRepretamo { get; set; }
    public int NEstado { get; init; }
    public string? Estado { get; init; }
    public int NSubProd { get; init; }
    public string? SubProducto { get; init; }
}
