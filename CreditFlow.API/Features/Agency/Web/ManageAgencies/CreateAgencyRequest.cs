using System.ComponentModel.DataAnnotations;

namespace CreditFlow.API.Features.Agency.Web.ManageAgencies;

public class CreateAgencyRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "El código de agencia debe ser mayor que cero.")]
    public int NCodAge { get; set; }

    [Required(ErrorMessage = "El nombre de la agencia es requerido.")]
    public string Nombre { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
    public string CorreoElectronico { get; set; } = string.Empty;
}
