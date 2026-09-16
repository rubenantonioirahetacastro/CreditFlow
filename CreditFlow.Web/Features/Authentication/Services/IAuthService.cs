using CreditFlow.Web.Features.Authentication.Models;

namespace CreditFlow.Web.Features.Authentication.Services;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(string documento, string password);
}
