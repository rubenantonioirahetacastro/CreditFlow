using System.ComponentModel.DataAnnotations;

namespace CreditFlow.API.Features.Authentication.Shared.Login
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "El documento es obligatorio.")]
        public string Documento { get; set; } = null!;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Password { get; set; } = null!;
    }
}
