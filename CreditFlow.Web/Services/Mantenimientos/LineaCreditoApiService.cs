using CreditFlow.Web.Core.Http;
using CreditFlow.Web.Models.Mantenimientos;

namespace CreditFlow.Web.Services.Mantenimientos;

public sealed class LineaCreditoApiService(IApiClient apiClient) : ILineaCreditoService
{
    private const string BaseUrl = "api/mantenimientos/lineas-credito";

    public async Task<List<LineaCreditoDto>> ObtenerTodasAsync()
    {
        var result = await apiClient.GetAsync<List<LineaCreditoDto>>(
            BaseUrl,
            "No se pudieron cargar las líneas de crédito.");
        return result.Data ?? [];
    }

    public async Task<(bool Exito, string? Mensaje)> CrearAsync(CrearLineaCreditoRequest request)
    {
        var result = await apiClient.PostAsync(BaseUrl, request, "No se pudo crear la línea de crédito.");
        return (result.IsSuccess, result.Message);
    }

    public async Task<(bool Exito, string? Mensaje)> ActualizarAsync(int id, ActualizarLineaCreditoRequest request)
    {
        var result = await apiClient.PutAsync(
            $"{BaseUrl}/{id}",
            request,
            "No se pudo actualizar la línea de crédito.");
        return (result.IsSuccess, result.Message);
    }

    public async Task<(bool Exito, string? Mensaje)> EliminarAsync(int id)
    {
        var result = await apiClient.DeleteAsync(
            $"{BaseUrl}/{id}",
            "No se pudo eliminar la línea de crédito.");
        return (result.IsSuccess, result.Message);
    }
}
