using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CreditFlow.Web.Models;
using CreditFlow.Web.Shared.CatalogoCodigos.Models;
using CreditFlow.Web.Services;

namespace CreditFlow.Web.Services.Mantenimientos;

public class CatalogoCodigoApiService : ICatalogoCodigoService
{
    private readonly HttpClient _httpClient;
    private readonly CustomAuthStateProvider _authStateProvider;

    public CatalogoCodigoApiService(IHttpClientFactory httpClientFactory, CustomAuthStateProvider authStateProvider)
    {
        _httpClient = httpClientFactory.CreateClient("CreditFlowApi");
        _authStateProvider = authStateProvider;
    }

    public async Task<List<CatalogoCodigoDto>> ObtenerTodosAsync()
    {
        try
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, "api/CatalogoCodigo");
            await AttachTokenAsync(httpRequest);

            var response = await _httpClient.SendAsync(httpRequest);
            if (!response.IsSuccessStatusCode)
                return new List<CatalogoCodigoDto>();

            var catalogos = await response.Content.ReadFromJsonAsync<List<CatalogoCodigoDto>>();
            return catalogos ?? new List<CatalogoCodigoDto>();
        }
        catch (Exception ex) when (ex is HttpRequestException or JsonException)
        {
            return new List<CatalogoCodigoDto>();
        }
    }

    public async Task<(bool Exito, string? Mensaje)> CrearAsync(CatalogoCodigoDto catalogo)
    {
        try
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/CatalogoCodigo")
            {
                Content = JsonContent.Create(catalogo)
            };
            await AttachTokenAsync(httpRequest);

            var response = await _httpClient.SendAsync(httpRequest);
            if (response.IsSuccessStatusCode)
                return (true, null);

            return (false, await ExtraerMensajeErrorAsync(response, "No se pudo crear el código de catálogo."));
        }
        catch (HttpRequestException)
        {
            return (false, "No se pudo conectar con el servidor. Intente nuevamente más tarde.");
        }
        catch (JsonException)
        {
            return (false, "El servidor respondió de forma inesperada. Intente nuevamente más tarde.");
        }
    }

    public async Task<(bool Exito, string? Mensaje)> ActualizarAsync(CatalogoCodigoDto catalogo)
    {
        try
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Put, "api/CatalogoCodigo")
            {
                Content = JsonContent.Create(catalogo)
            };
            await AttachTokenAsync(httpRequest);

            var response = await _httpClient.SendAsync(httpRequest);
            if (response.IsSuccessStatusCode)
                return (true, null);

            return (false, await ExtraerMensajeErrorAsync(response, "No se pudo actualizar el código de catálogo."));
        }
        catch (HttpRequestException)
        {
            return (false, "No se pudo conectar con el servidor. Intente nuevamente más tarde.");
        }
        catch (JsonException)
        {
            return (false, "El servidor respondió de forma inesperada. Intente nuevamente más tarde.");
        }
    }

    public async Task<(bool Exito, string? Mensaje)> EliminarAsync(int nCodigo, int nValor)
    {
        try
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"api/CatalogoCodigo/{nCodigo}/{nValor}");
            await AttachTokenAsync(httpRequest);

            var response = await _httpClient.SendAsync(httpRequest);
            if (response.IsSuccessStatusCode)
                return (true, null);

            return (false, await ExtraerMensajeErrorAsync(response, "No se pudo eliminar el código de catálogo."));
        }
        catch (HttpRequestException)
        {
            return (false, "No se pudo conectar con el servidor. Intente nuevamente más tarde.");
        }
        catch (JsonException)
        {
            return (false, "El servidor respondió de forma inesperada. Intente nuevamente más tarde.");
        }
    }

    private static async Task<string> ExtraerMensajeErrorAsync(HttpResponseMessage response, string mensajePorDefecto)
    {
        try
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            if (!string.IsNullOrWhiteSpace(error?.Mensaje))
                return error.Mensaje;
        }
        catch (JsonException)
        {
        }

        return response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden
            ? "No tenés permiso para realizar esta acción."
            : mensajePorDefecto;
    }

    private async Task AttachTokenAsync(HttpRequestMessage request)
    {
        var token = await _authStateProvider.ObtenerAccessTokenAsync();
        if (!string.IsNullOrEmpty(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
