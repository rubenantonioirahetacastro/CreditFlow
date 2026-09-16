namespace CreditFlow.Web.Features.Authentication.Models;

public class LoginRequestDto
{
    public string Documento { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}
