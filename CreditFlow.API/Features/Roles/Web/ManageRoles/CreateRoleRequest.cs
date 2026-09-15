using System.ComponentModel.DataAnnotations;

namespace CreditFlow.API.Features.Roles.Web.ManageRoles
{
    public class CreateRoleRequest
    {
        [Required(ErrorMessage = "El nombre del rol es requerido.")]
        public string Nombre { get; set; } = null!;

        public string? Descripcion { get; set; }
    }
}
