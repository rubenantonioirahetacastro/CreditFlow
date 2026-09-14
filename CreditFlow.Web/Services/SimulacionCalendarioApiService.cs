using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CreditFlow.Web.Models;

namespace CreditFlow.Web.Services;

public class SimulacionCalendarioApiService : ISimulacionCalendarioService
{
    private readonly HttpClient _httpClient;
    private readonly CustomAuthStateProvider _authStateProvider;

    public SimulacionCalendarioApiService(IHttpClientFactory httpClientFactory, CustomAuthStateProvider authStateProvider)
    {
        _httpClient = httpClientFactory.CreateClient("CreditFlowApi");
        _authStateProvider = authStateProvider;
    }

    public async Task<(bool Exito, string? Mensaje, SimularCalendarioResponse? Data)> SimularAsync(SimularCalendarioRequest request)
    {
        try
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/Credito/simular-calendario")
            {
                Content = JsonContent.Create(request)
            };
            await AttachTokenAsync(httpRequest);

            var response = await _httpClient.SendAsync(httpRequest);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<SimularCalendarioResponse>();
                return (true, null, data);
            }

            return (false, await ExtraerMensajeErrorAsync(response, "No se pudo generar el cronograma."), null);
        }
        catch (HttpRequestException)
        {
            return (false, "No se pudo conectar con el servidor. Intente nuevamente más tarde.", null);
        }
        catch (JsonException)
        {
            return (false, "El servidor respondió de forma inesperada. Intente nuevamente más tarde.", null);
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
