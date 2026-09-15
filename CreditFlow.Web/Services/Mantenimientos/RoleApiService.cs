using CreditFlow.Web.Core.Http;
using CreditFlow.Web.Models;
using CreditFlow.Web.Models.Mantenimientos;

namespace CreditFlow.Web.Services.Mantenimientos;

public sealed class RoleApiService(IApiClient apiClient) : IRoleService
{
    private const string BaseUrl = "api/mantenimientos/roles";

    public async Task<List<RoleDto>> ObtenerTodosAsync()
    {
        var result = await apiClient.GetAsync<List<RoleDto>>(
            $"{BaseUrl}/todos",
            "No se pudieron cargar los roles.");
        return result.Data ?? [];
    }

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
