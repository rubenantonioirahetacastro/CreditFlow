using System.Net.Http.Json;
using CreditFlow.Web.Models;

namespace CreditFlow.Web.Services;

public class AuthApiService : IAuthService
{
    private readonly HttpClient _httpClient;

    public AuthApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("CreditFlowApi");
    }

    public async Task<LoginResponseDto> LoginAsync(string documento, string password)
    {
        var request = new LoginRequestDto { Documento = documento, Password = password };

        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login-web", request);
            var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

            return result ?? new LoginResponseDto
            {
                Exito = false,
                Mensaje = "No se pudo interpretar la respuesta del servidor."
            };
        }
        catch (HttpRequestException)
        {
            return new LoginResponseDto
            {
                Exito = false,
                Mensaje = "No se pudo conectar con el servidor. Intente nuevamente más tarde."
            };
        }
    }

    public async Task<List<RoleDto>> ObtenerRolesAsync()
    {
        var roles = await _httpClient.GetFromJsonAsync<List<RoleDto>>("api/mantenimientos/roles");
        return roles ?? new List<RoleDto>();
    }
}
