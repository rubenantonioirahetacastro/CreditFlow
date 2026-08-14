namespace CreditFlow.Web.Features.Auth.Models;

public class LoginResponseDto
{
    public bool Exito { get; set; }

    public string? Mensaje { get; set; }

    public string? Token { get; set; }

    public int IdPersona { get; set; }

    public bool BTemporal { get; set; }

    public int IdRol { get; set; }
}
