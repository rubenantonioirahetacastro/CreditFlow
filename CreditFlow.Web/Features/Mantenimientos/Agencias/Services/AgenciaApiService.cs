using CreditFlow.Web.Core.Http;
using CreditFlow.Web.Features.Mantenimientos.Agencias.Models;

namespace CreditFlow.Web.Features.Mantenimientos.Agencias.Services;

public class AgenciaApiService : IAgenciaService
{
    private readonly IApiClient _apiClient;

    public AgenciaApiService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<AgenciaDto>> ObtenerTodasAsync()
    {
        var agencias = await _apiClient.GetAsync<List<AgenciaDto>>("api/mantenimientos/agencias");
        return agencias ?? new List<AgenciaDto>();
    }

    public Task<(bool Exito, string? Mensaje)> CrearAsync(CrearAgenciaRequest request)
        => _apiClient.PostAsync("api/mantenimientos/agencias", request);

    public Task<(bool Exito, string? Mensaje)> ActualizarAsync(int id, ActualizarAgenciaRequest request)
        => _apiClient.PutAsync($"api/mantenimientos/agencias/{id}", request);

    public Task<(bool Exito, string? Mensaje)> EliminarAsync(int id)
        => _apiClient.DeleteAsync($"api/mantenimientos/agencias/{id}");
}
