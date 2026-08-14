using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CreditFlow.Web.Shared.Models;
using CreditFlow.Web.Services;

namespace CreditFlow.Web.Core.Http;

public class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;
    private readonly CustomAuthStateProvider _authStateProvider;

    public ApiClient(IHttpClientFactory httpClientFactory, CustomAuthStateProvider authStateProvider)
    {
        _httpClient = httpClientFactory.CreateClient("CreditFlowApi");
        _authStateProvider = authStateProvider;
    }

    public async Task<T?> GetAsync<T>(string url)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            await AttachTokenAsync(request);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return default;

            return await response.Content.ReadFromJsonAsync<T>();
        }
        catch (Exception ex) when (ex is HttpRequestException or JsonException)
        {
            return default;
        }
    }

    public Task<(bool Exito, string? Mensaje)> PutAsync(string url, object? body = null)
        => EnviarAsync(HttpMethod.Put, url, body);

    public Task<(bool Exito, string? Mensaje)> PostAsync(string url, object? body = null)
        => EnviarAsync(HttpMethod.Post, url, body);

    public Task<(bool Exito, string? Mensaje)> DeleteAsync(string url)
        => EnviarAsync(HttpMethod.Delete, url, null);

    private async Task<(bool Exito, string? Mensaje)> EnviarAsync(HttpMethod metodo, string url, object? body)
    {
        try
        {
            var request = new HttpRequestMessage(metodo, url);
            if (body is not null)
                request.Content = JsonContent.Create(body);

            await AttachTokenAsync(request);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
                return (true, null);

            return (false, await ExtraerMensajeErrorAsync(response));
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

    private static async Task<string> ExtraerMensajeErrorAsync(HttpResponseMessage response)
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
            : "Ocurrió un error al procesar la solicitud.";
    }

    private async Task AttachTokenAsync(HttpRequestMessage request)
    {
        var token = await _authStateProvider.ObtenerAccessTokenAsync();
        if (!string.IsNullOrEmpty(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
