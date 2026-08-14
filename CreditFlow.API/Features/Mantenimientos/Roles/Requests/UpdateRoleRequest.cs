namespace CreditFlow.API.Application.Requests.Mantenimientos
{
    public class UpdateRoleRequest
    {
        public string Nombre { get; set; } = null!;

        public string? Descripcion { get; set; }

        public bool Activo { get; set; }
    }
}
