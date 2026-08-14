using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CreditFlow.Web.Shared.Models;
using CreditFlow.Web.Features.Mantenimientos.Empleados.Models;
using CreditFlow.Web.Services;
using Microsoft.AspNetCore.Components.Forms;

namespace CreditFlow.Web.Features.Mantenimientos.Empleados.Services;

public class EmpleadoApiService : IEmpleadoService
{
    private const long TamanoMaximoFotoBytes = 5 * 1024 * 1024; // 5MB

    private readonly HttpClient _httpClient;
    private readonly CustomAuthStateProvider _authStateProvider;

    public EmpleadoApiService(IHttpClientFactory httpClientFactory, CustomAuthStateProvider authStateProvider)
    {
        _httpClient = httpClientFactory.CreateClient("CreditFlowApi");
        _authStateProvider = authStateProvider;
    }

    public async Task<List<EmpleadoDto>> ObtenerTodosAsync()
    {
        try
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, "api/mantenimientos/empleados");
            await AttachTokenAsync(httpRequest);

            var response = await _httpClient.SendAsync(httpRequest);
            if (!response.IsSuccessStatusCode)
                return new List<EmpleadoDto>();

            var empleados = await response.Content.ReadFromJsonAsync<List<EmpleadoDto>>();
            return empleados ?? new List<EmpleadoDto>();
        }
        catch (Exception ex) when (ex is HttpRequestException or JsonException)
        {
            return new List<EmpleadoDto>();
        }
    }

    public async Task<(bool Exito, string? Mensaje)> CrearAsync(CrearEmpleadoRequest request)
    {
        try
        {
            var content = new MultipartFormDataContent
            {
                { new StringContent(request.Documento), nameof(request.Documento) },
                { new StringContent(request.Nombres), nameof(request.Nombres) },
                { new StringContent(request.PrimerApellido), nameof(request.PrimerApellido) },
                { new StringContent(request.SegundoApellido), nameof(request.SegundoApellido) },
                { new StringContent(request.Sexo.ToString()), nameof(request.Sexo) },
                { new StringContent(request.CodAgencia.ToString()), nameof(request.CodAgencia) },
                { new StringContent(request.Correo), nameof(request.Correo) },
                { new StringContent(request.Telefono), nameof(request.Telefono) },
                { new StringContent(request.Password), nameof(request.Password) },
                { new StringContent(request.IdRol.ToString()), nameof(request.IdRol) }
            };
            AgregarFoto(content, request.Foto);

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/mantenimientos/empleados")
            {
                Content = content
            };
            await AttachTokenAsync(httpRequest);

            var response = await _httpClient.SendAsync(httpRequest);
            if (response.IsSuccessStatusCode)
                return (true, null);

            return (false, await ExtraerMensajeErrorAsync(response, "No se pudo crear el empleado."));
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

    public async Task<(bool Exito, string? Mensaje)> ActualizarAsync(int id, ActualizarEmpleadoRequest request)
    {
        try
        {
            var content = new MultipartFormDataContent
            {
                { new StringContent(request.Nombres), nameof(request.Nombres) },
                { new StringContent(request.PrimerApellido), nameof(request.PrimerApellido) },
                { new StringContent(request.SegundoApellido), nameof(request.SegundoApellido) },
                { new StringContent(request.Sexo.ToString()), nameof(request.Sexo) },
                { new StringContent(request.CodAgencia.ToString()), nameof(request.CodAgencia) },
                { new StringContent(request.Correo), nameof(request.Correo) },
                { new StringContent(request.Telefono), nameof(request.Telefono) },
                { new StringContent(request.Estado.ToString()), nameof(request.Estado) },
                { new StringContent(request.IdRol.ToString()), nameof(request.IdRol) }
            };
            AgregarFoto(content, request.Foto);

            var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"api/mantenimientos/empleados/{id}")
            {
                Content = content
            };
            await AttachTokenAsync(httpRequest);

            var response = await _httpClient.SendAsync(httpRequest);
            if (response.IsSuccessStatusCode)
                return (true, null);

            return (false, await ExtraerMensajeErrorAsync(response, "No se pudo actualizar el empleado."));
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

    public async Task<(bool Exito, string? Mensaje)> EliminarAsync(int id)
    {
        try
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"api/mantenimientos/empleados/{id}");
            await AttachTokenAsync(httpRequest);

            var response = await _httpClient.SendAsync(httpRequest);
            if (response.IsSuccessStatusCode)
                return (true, null);

            return (false, await ExtraerMensajeErrorAsync(response, "No se pudo eliminar el empleado."));
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

    public async Task<string?> ObtenerFotoDataUrlAsync(int idEmpleado)
    {
        try
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"api/mantenimientos/empleados/{idEmpleado}/foto");
            await AttachTokenAsync(httpRequest);

            var response = await _httpClient.SendAsync(httpRequest);
            if (!response.IsSuccessStatusCode)
                return null;

            var bytes = await response.Content.ReadAsByteArrayAsync();
            var contentType = response.Content.Headers.ContentType?.MediaType ?? "image/jpeg";
            return $"data:{contentType};base64,{Convert.ToBase64String(bytes)}";
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    private static void AgregarFoto(MultipartFormDataContent content, IBrowserFile? foto)
    {
        if (foto == null)
            return;

        var stream = foto.OpenReadStream(TamanoMaximoFotoBytes);
        var streamContent = new StreamContent(stream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(foto.ContentType);
        content.Add(streamContent, "Foto", foto.Name);
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
