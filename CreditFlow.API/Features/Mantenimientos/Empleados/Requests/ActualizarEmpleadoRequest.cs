using Microsoft.AspNetCore.Http;

namespace CreditFlow.API.Features.Mantenimientos.Empleados.Requests
{
    public class ActualizarEmpleadoRequest
    {
        public string Nombres { get; set; } = string.Empty;

        public string PrimerApellido { get; set; } = string.Empty;

        public string SegundoApellido { get; set; } = string.Empty;

        public int Sexo { get; set; }

        public int CodAgencia { get; set; }

        public string Correo { get; set; } = string.Empty;

        public string Telefono { get; set; } = string.Empty;

        public int Estado { get; set; }

        public int IdRol { get; set; }

        public IFormFile? Foto { get; set; }
    }
}
