using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CreditFlow.API.Features.Verification.Web.GetCreditRequests;

[Route("api/BandejaVerificacion")]
[ApiController]
[Authorize]
public sealed class BandejaVerificacionController(
    IObtenerBandejaVerificacionHandler handler) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Obtener([FromQuery] int? nCodAge)
        => Ok(await handler.EjecutarAsync(nCodAge));
}
