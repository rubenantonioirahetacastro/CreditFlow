using CreditFlow.Web.Models;

namespace CreditFlow.Web.Services;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(string documento, string password);

    Task<List<RoleDto>> ObtenerRolesAsync();
}
