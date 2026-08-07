using CreditFlow.API.Application.DTOs.Mantenimientos;

namespace CreditFlow.API.Application.Interfaces.Mantenimientos
{
    public interface IRoleService
    {
        Task<List<RoleDto>> ObtenerActivosAsync();
    }
}
