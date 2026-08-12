using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CreditFlow.Web.Models;
using CreditFlow.Web.Models.Mantenimientos;
using CreditFlow.Web.Services;

namespace CreditFlow.Web.Services.Mantenimientos;

public class RoleApiService : IRoleService
{
    private readonly HttpClient _httpClient;
    private readonly CustomAuthStateProvider _authStateProvider;

    public RoleApiService(IHttpClientFactory httpClientFactory, CustomAuthStateProvider authStateProvider)
    {
        _httpClient = httpClientFactory.CreateClient("CreditFlowApi");
        _authStateProvider = authStateProvider;
    }

    public async Task<List<RoleDto>> ObtenerTodosAsync()
    {
        try
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, "api/mantenimientos/roles/todos");
            await AttachTokenAsync(httpRequest);

            var response = await _httpClient.SendAsync(httpRequest);
            if (!response.IsSuccessStatusCode)
                return new List<RoleDto>();

            var roles = await response.Content.ReadFromJsonAsync<List<RoleDto>>();
            return roles ?? new List<RoleDto>();
        }
        catch (Exception ex) when (ex is HttpRequestException or JsonException)
        {
            return new List<RoleDto>();
        }
    }

    public async Task<(bool Exito, string? Mensaje)> CrearAsync(CrearRoleRequest request)
    {
        try
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/mantenimientos/roles")
            {
                Content = JsonContent.Create(request)
            };
            await AttachTokenAsync(httpRequest);

            var response = await _httpClient.SendAsync(httpRequest);
            if (response.IsSuccessStatusCode)
                return (true, null);

            return (false, await ExtraerMensajeErrorAsync(response, "No se pudo crear el rol."));
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

    public async Task<(bool Exito, string? Mensaje)> ActualizarAsync(int id, ActualizarRoleRequest request)
    {
        try
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"api/mantenimientos/roles/{id}")
            {
                Content = JsonContent.Create(request)
            };
            await AttachTokenAsync(httpRequest);

            var response = await _httpClient.SendAsync(httpRequest);
            if (response.IsSuccessStatusCode)
                return (true, null);

            return (false, await ExtraerMensajeErrorAsync(response, "No se pudo actualizar el rol."));
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
