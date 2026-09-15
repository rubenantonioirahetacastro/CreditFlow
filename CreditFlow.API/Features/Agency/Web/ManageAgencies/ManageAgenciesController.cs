using CreditFlow.API.Core.Security;
using CreditFlow.API.Core.Errors;
using CreditFlow.API.Features.Agency.Shared.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CreditFlow.API.Features.Agency.Web.ManageAgencies;

[Route("api/mantenimientos/agencias")]
[ApiController]
public class ManageAgenciesController(IAgencyService service) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.Maintenance)]
    public async Task<IActionResult> GetAll() => Ok(await service.GetAllAsync());

    [HttpGet("{id}")]
    [Authorize(Policy = AuthorizationPolicies.Maintenance)]
    public async Task<IActionResult> GetById(int id)
    {
        var agency = await service.GetByIdAsync(id);
        if (agency == null)
            throw new ResourceNotFoundException(AgencyErrors.NotFound);

        return Ok(agency);
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.Administration)]
    public async Task<IActionResult> Create([FromBody] CreateAgencyRequest request)
    {
        var agency = await service.CreateAsync(request);
        return Created($"api/mantenimientos/agencias/{agency.NCodAge}", agency);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = AuthorizationPolicies.Administration)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAgencyRequest request)
    {
        var agency = await service.UpdateAsync(id, request);
        if (agency == null)
            throw new ResourceNotFoundException(AgencyErrors.NotFound);

        return Ok(agency);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = AuthorizationPolicies.Administration)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await service.DeleteAsync(id);
        if (!deleted)
            throw new ResourceNotFoundException(AgencyErrors.NotFound);

        return NoContent();
    }
}
