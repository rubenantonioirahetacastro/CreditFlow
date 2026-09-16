using Microsoft.AspNetCore.Components.Authorization;

namespace CreditFlow.Web.Core.Security;

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
        return state.User.FindFirst(CustomClaimTypes.AccessToken)?.Value;
    }
}
