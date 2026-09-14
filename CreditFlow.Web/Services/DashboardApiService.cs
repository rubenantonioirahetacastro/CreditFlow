using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CreditFlow.Web.Models;

namespace CreditFlow.Web.Services;

public class DashboardApiService : IDashboardService
{
    private readonly HttpClient _httpClient;
    private readonly CustomAuthStateProvider _authStateProvider;

    public DashboardApiService(IHttpClientFactory httpClientFactory, CustomAuthStateProvider authStateProvider)
    {
        _httpClient = httpClientFactory.CreateClient("CreditFlowApi");
        _authStateProvider = authStateProvider;
    }

    public async Task<DashboardResumenResponse?> ObtenerResumenAsync()
    {
        try
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, "api/Dashboard/resumen");
            var token = await _authStateProvider.ObtenerAccessTokenAsync();
            if (!string.IsNullOrEmpty(token))
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(httpRequest);
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<DashboardResumenResponse>();
        }
        catch (Exception ex) when (ex is HttpRequestException or JsonException)
        {
            return null;
        }
    }
}
