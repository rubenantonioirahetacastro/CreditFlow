namespace CreditFlow.API.Application.DTOs.Mantenimientos
{
    public class AgenciaDto
    {
        public int NCodAge { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Direccion { get; set; } = string.Empty;

        public string Telefono { get; set; } = string.Empty;

        public string CorreoElectronico { get; set; } = string.Empty;
    }
}
