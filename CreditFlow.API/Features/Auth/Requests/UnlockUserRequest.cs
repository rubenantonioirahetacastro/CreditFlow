namespace CreditFlow.API.Features.Auth.Requests
{
    public class UnlockUserRequest
    {
        public string Usuario { get; set; } = null!;
        public string? Observacion { get; set; }
    }
}
