using CreditFlow.Web.Core.Http;
using CreditFlow.Web.Features.Maintenance.Role.Models;

namespace CreditFlow.Web.Features.Maintenance.Role.Services;

public sealed class RoleApiService(IApiClient apiClient) : IRoleService
{
    private const string BaseUrl = "api/mantenimientos/roles";

    public Task<ApiResult<List<RoleDto>>> ObtenerTodosAsync() =>
        apiClient.GetAsync<List<RoleDto>>(
            $"{BaseUrl}/todos",
            "No se pudieron cargar los roles.");

    public async Task<(bool Exito, string? Mensaje)> CrearAsync(CrearRoleRequest request)
    {
        var result = await apiClient.PostAsync(BaseUrl, request, "No se pudo crear el rol.");
        return (result.IsSuccess, result.Message);
    }

    public async Task<(bool Exito, string? Mensaje)> ActualizarAsync(int id, ActualizarRoleRequest request)
    {
        var result = await apiClient.PutAsync($"{BaseUrl}/{id}", request, "No se pudo actualizar el rol.");
        return (result.IsSuccess, result.Message);
    }
}
