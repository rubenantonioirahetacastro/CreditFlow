namespace CreditFlow.API.Features.Authentication.Mobile.UnlockUser;

public class UnlockConfirmRequest
{
    public string Usuario { get; set; } = null!;
    public string Token { get; set; } = null!;
}
