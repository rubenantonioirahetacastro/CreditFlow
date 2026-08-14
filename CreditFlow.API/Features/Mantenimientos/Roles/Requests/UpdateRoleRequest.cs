namespace CreditFlow.API.Features.Mantenimientos.Roles.Requests
{
    public class UpdateRoleRequest
    {
        public string Nombre { get; set; } = null!;

        public string? Descripcion { get; set; }

        public bool Activo { get; set; }
    }
}
