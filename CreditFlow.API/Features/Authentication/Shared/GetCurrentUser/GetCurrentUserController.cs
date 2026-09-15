using System.Security.Claims;
using CreditFlow.API.Core.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CreditFlow.API.Features.Authentication.Shared.GetCurrentUser;

[Route("api/Auth")]
[ApiController]
public class GetCurrentUserController : ControllerBase
{
    [HttpGet("me")]
    [Authorize]
    public IActionResult GetCurrentUser()
    {
        var name = User.Identity?.Name ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var claims = User.Claims
            .Select(claim => new { Type = claim.Type, Value = claim.Value })
            .ToList();
        var roleId = User.FindFirst(CustomClaimTypes.RoleId)?.Value;

        return Ok(new { Usuario = name, IdRol = roleId, Claims = claims });
    }
}
