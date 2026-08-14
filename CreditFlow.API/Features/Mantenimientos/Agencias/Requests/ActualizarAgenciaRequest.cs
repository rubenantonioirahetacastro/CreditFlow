namespace CreditFlow.API.Features.Mantenimientos.Agencias.Requests
{
    public class ActualizarAgenciaRequest
    {
        public string Nombre { get; set; } = string.Empty;

        public string Direccion { get; set; } = string.Empty;

        public string Telefono { get; set; } = string.Empty;

        public string CorreoElectronico { get; set; } = string.Empty;
    }
}
