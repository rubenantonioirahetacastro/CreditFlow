namespace CreditFlow.API.Features.Authentication.Web.UnlockUser;

public class UnlockUserRequest
{
    public string Usuario { get; set; } = null!;
    public string? Observacion { get; set; }
}
