using CreditFlow.Web.Core.Http;
using CreditFlow.Web.Models.Mantenimientos;

namespace CreditFlow.Web.Services.Mantenimientos;

public sealed class AgenciaApiService(IApiClient apiClient) : IAgenciaService
{
    private const string BaseUrl = "api/mantenimientos/agencias";

    public async Task<List<AgenciaDto>> ObtenerTodasAsync()
    {
        var result = await apiClient.GetAsync<List<AgenciaDto>>(
            BaseUrl,
            "No se pudieron cargar las agencias.");
        return result.Data ?? [];
    }

    public async Task<(bool Exito, string? Mensaje)> CrearAsync(CrearAgenciaRequest request)
    {
        var result = await apiClient.PostAsync(BaseUrl, request, "No se pudo crear la agencia.");
        return (result.IsSuccess, result.Message);
    }

    public async Task<(bool Exito, string? Mensaje)> ActualizarAsync(int id, ActualizarAgenciaRequest request)
    {
        var result = await apiClient.PutAsync($"{BaseUrl}/{id}", request, "No se pudo actualizar la agencia.");
        return (result.IsSuccess, result.Message);
    }

    public async Task<(bool Exito, string? Mensaje)> EliminarAsync(int id)
    {
        var result = await apiClient.DeleteAsync($"{BaseUrl}/{id}", "No se pudo eliminar la agencia.");
        return (result.IsSuccess, result.Message);
    }
}
