using CreditFlow.API.Features.Mantenimientos.Roles.DTOs;
using CreditFlow.API.Features.Mantenimientos.Roles.Requests;

namespace CreditFlow.API.Features.Mantenimientos.Roles.Services
{
    public interface IRoleService
    {
        Task<List<RoleDto>> ObtenerActivosAsync();

        Task<List<RoleDto>> ObtenerTodosAsync();

        Task<RoleDto> CrearAsync(CreateRoleRequest request);

        Task<RoleDto?> ActualizarAsync(int idRol, UpdateRoleRequest request);
    }
}
