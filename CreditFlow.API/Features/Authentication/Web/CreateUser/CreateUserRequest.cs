namespace CreditFlow.API.Features.Authentication.Web.CreateUser
{
    public class CreateUserRequest
    {
        public string Documento { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Correo { get; set; } = string.Empty;
        public int IdRol { get; set; }
    }
}
