using System.ComponentModel.DataAnnotations;

namespace CreditFlow.API.Features.Roles.Web.ManageRoles
{
    public class UpdateRoleRequest
    {
        [Required(ErrorMessage = "El nombre del rol es requerido.")]
        public string Nombre { get; set; } = null!;

        public string? Descripcion { get; set; }

        public bool Activo { get; set; }
    }
}
