namespace CreditFlow.API.Application.DTOs.Mantenimientos
{
    public class RoleDto
    {
        public int IdRol { get; set; }

        public string Nombre { get; set; } = null!;

        public string? Descripcion { get; set; }

        public bool Activo { get; set; }
    }
}
