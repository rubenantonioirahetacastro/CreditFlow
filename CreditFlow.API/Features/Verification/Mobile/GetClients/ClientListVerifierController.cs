using CreditFlow.API.Core.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CreditFlow.API.Features.Verification.Mobile.GetClients;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = AuthorizationPolicies.MobileVerifier)]
public sealed class ClientListVerifierController(
    IObtenerClientListVerifierHandler handler) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Obtener()
        => Ok(await handler.EjecutarAsync());
}
