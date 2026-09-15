using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CreditFlow.API.Features.Credit.Web.UpdateCreditDecision;

[Route("api/Credito")]
[ApiController]
[Authorize]
public sealed class UpdateCreditDecisionController(
    IUpdateCreditDecisionHandler handler) : ControllerBase
{
    [HttpPut("actualizar-evaluacion")]
    public async Task<IActionResult> Update(
        [FromBody] UpdateCreditDecisionRequest request,
        CancellationToken cancellationToken)
    {
        await handler.ExecuteAsync(request, cancellationToken);
        return NoContent();
    }
}
