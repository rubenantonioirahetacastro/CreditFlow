using CreditFlow.Web.Core.Http;
using CreditFlow.Web.Models;

namespace CreditFlow.Web.Services;

public sealed class AuthApiService(IApiClient apiClient) : IAuthService
{
    public async Task<LoginResponseDto> LoginAsync(string documento, string password)
    {
        var request = new LoginRequestDto { Documento = documento, Password = password };
        var result = await apiClient.PostAnonymousAsync<LoginRequestDto, LoginResponseDto>(
            "api/auth/login-web",
            request,
            "No se pudo iniciar sesión.");

        return result.Data ?? new LoginResponseDto
        {
            Exito = false,
            Mensaje = result.Message ?? "No se pudo iniciar sesión."
        };
    }

    public async Task<List<RoleDto>> ObtenerRolesAsync()
    {
        var result = await apiClient.GetAsync<List<RoleDto>>(
            "api/mantenimientos/roles",
            "No se pudieron cargar los roles.");
        return result.Data ?? [];
    }
}
