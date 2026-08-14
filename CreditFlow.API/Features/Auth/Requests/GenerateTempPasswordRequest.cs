namespace CreditFlow.API.Features.Auth.Requests
{
    public class GenerateTempPasswordRequest
    {
        public string Usuario { get; set; } = null!;
        public bool EnviarCorreo { get; set; } = false;
    }
}
