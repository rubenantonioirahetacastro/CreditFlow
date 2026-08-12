using CreditFlow.Web.Models;
using CreditFlow.Web.Models.Mantenimientos;

namespace CreditFlow.Web.Services.Mantenimientos;

public interface IRoleService
{
    Task<List<RoleDto>> ObtenerTodosAsync();

    Task<(bool Exito, string? Mensaje)> CrearAsync(CrearRoleRequest request);

    Task<(bool Exito, string? Mensaje)> ActualizarAsync(int id, ActualizarRoleRequest request);
}
