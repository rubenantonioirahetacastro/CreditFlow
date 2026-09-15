using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CreditFlow.API.Features.Employee.Web.ManageEmployees
{
    public class CreateEmployeeRequest
    {
        [Required(ErrorMessage = "El documento es obligatorio.")]
        public string Documento { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los nombres son obligatorios.")]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "El primer apellido es obligatorio.")]
        public string PrimerApellido { get; set; } = string.Empty;

        public string SegundoApellido { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "El sexo es obligatorio.")]
        public int Sexo { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La agencia es obligatoria.")]
        public int CodAgencia { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo no es válido.")]
        public string Correo { get; set; } = string.Empty;

        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Password { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "El rol es obligatorio.")]
        public int IdRol { get; set; }

        public IFormFile? Foto { get; set; }
    }
}
