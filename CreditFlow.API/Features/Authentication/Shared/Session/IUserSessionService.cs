namespace CreditFlow.API.Features.Authentication.Shared.Session;

public interface IUserSessionService
{
    Task<UserSession> GetAsync(int userId, string document);
}
