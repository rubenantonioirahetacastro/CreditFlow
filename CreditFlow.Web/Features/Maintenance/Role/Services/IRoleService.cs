using CreditFlow.Web.Core.Http;
using CreditFlow.Web.Features.Maintenance.Role.Models;

namespace CreditFlow.Web.Features.Maintenance.Role.Services;

public interface IRoleService
{
    Task<ApiResult<List<RoleDto>>> ObtenerTodosAsync();

    Task<(bool Exito, string? Mensaje)> CrearAsync(CrearRoleRequest request);

    Task<(bool Exito, string? Mensaje)> ActualizarAsync(int id, ActualizarRoleRequest request);
}
