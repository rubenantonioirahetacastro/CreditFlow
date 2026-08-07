using System.Net.Http.Json;
using System.Text.Json;
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

        // Este es un boundary de red: además de no poder conectar (API caída,
        // HttpRequestException), la API puede responder con un 500 no-JSON
        // (p. ej. una excepción no controlada devuelta como texto plano en
        // Development) — ReadFromJsonAsync lanza JsonException en ese caso.
        // Cualquiera de los dos casos debe degradar a un mensaje amigable en
        // vez de propagar la excepción hacia el endpoint no interactivo /login.
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
        catch (JsonException)
        {
            return new LoginResponseDto
            {
                Exito = false,
                Mensaje = "El servidor respondió de forma inesperada. Intente nuevamente más tarde."
            };
        }
    }

    public async Task<List<RoleDto>> ObtenerRolesAsync()
    {
        // Se llama en el mismo camino crítico que LoginAsync (justo después de
        // un login exitoso, antes de firmar la cookie). Si falla, no debe
        // tumbar el login: se degrada a lista vacía, y AuthEndpoints ya cae de
        // vuelta al IdRol numérico como nombre de rol en ese caso.
        try
        {
            var roles = await _httpClient.GetFromJsonAsync<List<RoleDto>>("api/mantenimientos/roles");
            return roles ?? new List<RoleDto>();
        }
        catch (Exception ex) when (ex is HttpRequestException or JsonException)
        {
            return new List<RoleDto>();
        }
    }
}
