using CreditFlow.API.Core.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CreditFlow.API.Features.Simulator.Shared.SimulateCalendar;

[Route("api/Credito")]
[ApiController]
[Authorize(Policy = AuthorizationPolicies.CalendarConfiguration)]
public sealed class SimulateCalendarController(
    ISimulacionCalendarioService simulationService) : ControllerBase
{
    [HttpPost("simular-calendario")]
    public async Task<ActionResult<SimularCalendarioResponse>> Simulate(
        [FromBody] SimularCalendarioRequest request,
        CancellationToken cancellationToken)
    {
        var response = await simulationService.SimularAsync(request);
        return Ok(response);
    }
}
