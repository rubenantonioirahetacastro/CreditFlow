using CreditFlow.Web.Shared.Models;
using CreditFlow.Web.Features.Auth.Models;

namespace CreditFlow.Web.Features.Auth.Services;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(string documento, string password);

    Task<List<RoleDto>> ObtenerRolesAsync();
}
