using CreditFlow.Web.Core.Http;
using CreditFlow.Web.Shared.Models;
using CreditFlow.Web.Features.Mantenimientos.Roles.Models;

namespace CreditFlow.Web.Features.Mantenimientos.Roles.Services;

public class RoleApiService : IRoleService
{
    private readonly IApiClient _apiClient;

    public RoleApiService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<RoleDto>> ObtenerTodosAsync()
    {
        var roles = await _apiClient.GetAsync<List<RoleDto>>("api/mantenimientos/roles/todos");
        return roles ?? new List<RoleDto>();
    }

    public Task<(bool Exito, string? Mensaje)> CrearAsync(CrearRoleRequest request)
        => _apiClient.PostAsync("api/mantenimientos/roles", request);

    public Task<(bool Exito, string? Mensaje)> ActualizarAsync(int id, ActualizarRoleRequest request)
        => _apiClient.PutAsync($"api/mantenimientos/roles/{id}", request);
}
