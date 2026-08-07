namespace CreditFlow.API.Application.Requests.Mantenimientos
{
    public class CreateRoleRequest
    {
        public string Nombre { get; set; } = null!;

        public string? Descripcion { get; set; }
    }
}
