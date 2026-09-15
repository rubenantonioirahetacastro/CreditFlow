using CreditFlow.API.Core.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CreditFlow.API.Features.Employee.Mobile.GetEmployeeHome;

[Route("api/movil/inicio-empleado")]
[ApiController]
[Authorize(Policy = AuthorizationPolicies.MobileVerifier)]
public sealed class EmployeeHomeController(IGetEmployeeHomeHandler handler) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        if (!TryGetCurrentEmployee(out var employeeId, out var roleId))
            return Unauthorized();

        var home = await handler.ExecuteAsync(employeeId, roleId);
        return home == null
            ? NotFound(new { Mensaje = "No se encontró el empleado autenticado." })
            : Ok(home);
    }

    private bool TryGetCurrentEmployee(out int employeeId, out int roleId)
    {
        var employeeClaim = User.FindFirst(CustomClaimTypes.EmployeeId)?.Value;
        var roleClaim = User.FindFirst(CustomClaimTypes.RoleId)?.Value;
        employeeId = default;
        roleId = default;

        return int.TryParse(employeeClaim, out employeeId) &&
               int.TryParse(roleClaim, out roleId);
    }
}
