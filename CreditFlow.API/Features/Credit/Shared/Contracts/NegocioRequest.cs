namespace CreditFlow.API.Features.Credit.Shared.Contracts;
public class NegocioRequest
{
    public string CNombre { get; set; } = null!;

    public string CDireccion { get; set; } = null!;

    public int CSector { get; set; }

    public TimeOnly? THoraInicio { get; set; }

    public TimeOnly? THoraCierre { get; set; }

    public string CTelefono { get; set; } = null!;

    public string? CGeolocalizacion { get; set; }
}
