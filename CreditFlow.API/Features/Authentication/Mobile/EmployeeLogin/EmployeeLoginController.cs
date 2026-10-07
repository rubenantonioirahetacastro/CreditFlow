using Microsoft.AspNetCore.Mvc;

namespace CreditFlow.API.Features.Authentication.Mobile.EmployeeLogin;

[Route("api/Auth")]
[ApiController]
public sealed class EmployeeLoginController(EmployeeLoginHandler handler) : ControllerBase
{
    [HttpPost("login-empleado")]
    [ProducesResponseType(typeof(EmployeeLoginResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<EmployeeLoginResponse>> Login(
        [FromBody] EmployeeLoginRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await handler.ExecuteAsync(request, cancellationToken));
    }
}
