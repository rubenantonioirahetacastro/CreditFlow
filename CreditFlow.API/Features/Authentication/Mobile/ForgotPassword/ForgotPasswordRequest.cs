namespace CreditFlow.API.Features.Authentication.Mobile.ForgotPassword
{
    public class ForgotPasswordRequest
    {
        public string Usuario { get; set; } = null!;
        public bool EnviarCorreo { get; set; } = false;
    }
}
