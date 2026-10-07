using CreditFlow.Web.Core.Http;
using CreditFlow.Web.Features.Authentication.Models;

namespace CreditFlow.Web.Features.Authentication.Services;

public sealed class AuthApiService(IApiClient apiClient) : IAuthService
{
    public async Task<LoginResponseDto> LoginAsync(string documento, string password)
    {
        var request = new LoginRequestDto { Documento = documento, Password = password };
        var result = await apiClient.PostAnonymousAsync<LoginRequestDto, LoginResponseDto>(
            "api/auth/login-web",
            request,
            "No se pudo iniciar sesión.");

        return result.Data ?? new LoginResponseDto
        {
            Exito = false,
            Mensaje = result.Message ?? "No se pudo iniciar sesión."
        };
    }

    // Se pide una vez por circuito (el servicio es scoped): la barra superior se vuelve a dibujar en cada página.
    private Task<string?>? fotoPerfil;

    public Task<string?> ObtenerFotoPerfilAsync() => fotoPerfil ??= CargarFotoPerfilAsync();

    private async Task<string?> CargarFotoPerfilAsync()
    {
        var result = await apiClient.GetImageDataUrlAsync("api/auth/me/foto");
        return result.IsSuccess ? result.Data : null;
    }

}
