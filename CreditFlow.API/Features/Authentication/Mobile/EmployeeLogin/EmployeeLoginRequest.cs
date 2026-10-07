using System.ComponentModel.DataAnnotations;

namespace CreditFlow.API.Features.Authentication.Mobile.EmployeeLogin;

public sealed class EmployeeLoginRequest
{
    [Required(ErrorMessage = "El código de usuario es obligatorio.")]
    public string CCodUsu { get; set; } = null!;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    public string Password { get; set; } = null!;
}
