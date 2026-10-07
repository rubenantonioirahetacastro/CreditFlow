using CreditFlow.Web.Features.Authentication.Models;

namespace CreditFlow.Web.Features.Authentication.Services;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(string documento, string password);

    /// <summary>Foto de perfil del usuario autenticado como data URL; nula si no tiene o no se pudo cargar.</summary>
    Task<string?> ObtenerFotoPerfilAsync();
}
