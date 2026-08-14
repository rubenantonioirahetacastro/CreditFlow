namespace CreditFlow.API.Features.Mantenimientos.Agencias.Requests
{
    // NCodAge se asigna manualmente (Agencia.NCodAge usa ValueGeneratedNever() en
    // DbNegocioContext), no es autogenerado por la base de datos.
    public class CrearAgenciaRequest
    {
        public int NCodAge { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Direccion { get; set; } = string.Empty;

        public string Telefono { get; set; } = string.Empty;

        public string CorreoElectronico { get; set; } = string.Empty;
    }
}
