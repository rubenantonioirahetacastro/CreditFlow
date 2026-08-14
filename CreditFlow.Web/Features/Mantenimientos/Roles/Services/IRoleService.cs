using CreditFlow.Web.Shared.Models;
using CreditFlow.Web.Features.Mantenimientos.Roles.Models;

namespace CreditFlow.Web.Features.Mantenimientos.Roles.Services;

public interface IRoleService
{
    Task<List<RoleDto>> ObtenerTodosAsync();

    Task<(bool Exito, string? Mensaje)> CrearAsync(CrearRoleRequest request);

    Task<(bool Exito, string? Mensaje)> ActualizarAsync(int id, ActualizarRoleRequest request);
}
