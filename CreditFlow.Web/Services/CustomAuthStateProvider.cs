using Microsoft.AspNetCore.Components.Authorization;

namespace CreditFlow.Web.Services;

// El estado de autenticación real proviene de la cookie firmada por el
// endpoint no interactivo POST /login (Endpoints/AuthEndpoints.cs) y llega a
// cada circuito a través del AuthenticationStateProvider por defecto que
// registra AddCascadingAuthenticationState(), a partir de HttpContext.User.
// Esta clase es un accesor de conveniencia sobre ese estado (p. ej. para
// leer el AccessToken guardado como claim), no una fuente de verdad propia.
public class CustomAuthStateProvider
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public CustomAuthStateProvider(AuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationStateProvider = authenticationStateProvider;
    }

    public async Task<string?> ObtenerAccessTokenAsync()
    {
        var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
        return state.User.FindFirst("AccessToken")?.Value;
    }
}
