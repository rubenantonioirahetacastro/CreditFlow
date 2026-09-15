using CreditFlow.API.Features.Authentication.Shared.Session;

namespace CreditFlow.API.Features.Authentication.Shared.Token;

public interface IJwtTokenService
{
    string Create(UserSession session);
}
