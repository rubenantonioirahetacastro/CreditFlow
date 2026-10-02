using System.Net.Http.Headers;
using CreditFlow.Web.Core.Http;
using CreditFlow.Web.Features.Maintenance.Employee.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace CreditFlow.Web.Features.Maintenance.Employee.Services;

public sealed class EmpleadoApiService(IApiClient apiClient) : IEmpleadoService
{
    private const long TamanoMaximoFotoBytes = 5 * 1024 * 1024;
    private const string BaseUrl = "api/mantenimientos/empleados";

    public Task<ApiResult<List<EmpleadoDto>>> ObtenerTodosAsync() =>
        apiClient.GetAsync<List<EmpleadoDto>>(
            BaseUrl,
            "No se pudieron cargar los empleados.");

    // El contenido se tipa como HttpContent para usar la sobrecarga multipart de IApiClient;
    // con el tipo concreto, C# elige la sobrecarga genérica y lo enviaría como JSON.
    public async Task<(bool Exito, string? Mensaje)> CrearAsync(CrearEmpleadoRequest request)
    {
        HttpContent content = CrearContenido(request);
        var result = await apiClient.PostAsync(BaseUrl, content, "No se pudo crear el empleado.");
        return (result.IsSuccess, result.Message);
    }

    public async Task<(bool Exito, string? Mensaje)> ActualizarAsync(
        int id,
        ActualizarEmpleadoRequest request)
    {
        HttpContent content = CrearContenido(request);
        var result = await apiClient.PutAsync(
            $"{BaseUrl}/{id}",
            content,
            "No se pudo actualizar el empleado.");
        return (result.IsSuccess, result.Message);
    }

    public async Task<(bool Exito, string? Mensaje)> EliminarAsync(int id)
    {
        var result = await apiClient.DeleteAsync(
            $"{BaseUrl}/{id}",
            "No se pudo eliminar el empleado.");
        return (result.IsSuccess, result.Message);
    }

    public async Task<string?> ObtenerFotoDataUrlAsync(int idEmpleado)
    {
        var result = await apiClient.GetImageDataUrlAsync($"{BaseUrl}/{idEmpleado}/foto");
        return result.Data;
    }

    private static MultipartFormDataContent CrearContenido(CrearEmpleadoRequest request)
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
        return content;
    }

    private static MultipartFormDataContent CrearContenido(ActualizarEmpleadoRequest request)
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
        return content;
    }

    private static void AgregarFoto(MultipartFormDataContent content, IBrowserFile? foto)
    {
        if (foto is null)
            return;

        var streamContent = new StreamContent(foto.OpenReadStream(TamanoMaximoFotoBytes));
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(foto.ContentType);
        content.Add(streamContent, "Foto", foto.Name);
    }
}
