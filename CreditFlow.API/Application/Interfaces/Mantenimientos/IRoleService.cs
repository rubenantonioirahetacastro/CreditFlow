using CreditFlow.API.Application.DTOs.Mantenimientos;
using CreditFlow.API.Application.Requests.Mantenimientos;

namespace CreditFlow.API.Application.Interfaces.Mantenimientos
{
    public interface IRoleService
    {
        Task<List<RoleDto>> ObtenerActivosAsync();

        Task<RoleDto> CrearAsync(CreateRoleRequest request);

        Task<RoleDto?> ActualizarAsync(int idRol, UpdateRoleRequest request);
    }
}
