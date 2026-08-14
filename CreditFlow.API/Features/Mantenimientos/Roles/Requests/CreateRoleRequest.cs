namespace CreditFlow.API.Features.Mantenimientos.Roles.Requests
{
    public class CreateRoleRequest
    {
        public string Nombre { get; set; } = null!;

        public string? Descripcion { get; set; }
    }
}
