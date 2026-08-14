using CreditFlow.Web.Core.Http;
using CreditFlow.Web.Features.Mantenimientos.LineasCredito.Models;

namespace CreditFlow.Web.Features.Mantenimientos.LineasCredito.Services;

public class LineaCreditoApiService : ILineaCreditoService
{
    private readonly IApiClient _apiClient;

    public LineaCreditoApiService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<LineaCreditoDto>> ObtenerTodasAsync()
    {
        var lineas = await _apiClient.GetAsync<List<LineaCreditoDto>>("api/mantenimientos/lineas-credito");
        return lineas ?? new List<LineaCreditoDto>();
    }

    public Task<(bool Exito, string? Mensaje)> CrearAsync(CrearLineaCreditoRequest request)
        => _apiClient.PostAsync("api/mantenimientos/lineas-credito", request);

    public Task<(bool Exito, string? Mensaje)> ActualizarAsync(int id, ActualizarLineaCreditoRequest request)
        => _apiClient.PutAsync($"api/mantenimientos/lineas-credito/{id}", request);

    public Task<(bool Exito, string? Mensaje)> EliminarAsync(int id)
        => _apiClient.DeleteAsync($"api/mantenimientos/lineas-credito/{id}");
}
