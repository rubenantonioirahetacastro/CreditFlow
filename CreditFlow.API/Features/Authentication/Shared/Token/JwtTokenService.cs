using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CreditFlow.API.Core.Security;
using CreditFlow.API.Features.Authentication.Shared.Session;
using Microsoft.IdentityModel.Tokens;

namespace CreditFlow.API.Features.Authentication.Shared.Token;

public sealed class JwtTokenService(IConfiguration configuration) : IJwtTokenService
{
    public string Create(UserSession session)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, session.UserId.ToString()),
            new(ClaimTypes.Name, session.Document),
            new(CustomClaimTypes.Document, session.Document)
        };

        if (session.PersonId.HasValue)
            claims.Add(new Claim(CustomClaimTypes.PersonId, session.PersonId.Value.ToString()));

        if (session.EmployeeId.HasValue)
            claims.Add(new Claim(CustomClaimTypes.EmployeeId, session.EmployeeId.Value.ToString()));

        foreach (var roleId in session.RoleIds)
            claims.Add(new Claim(CustomClaimTypes.RoleId, roleId.ToString()));

        var jwtKey = configuration["Jwt:Key"]
            ?? "ChangeThisSecretInProduction_ReplaceMeWithStrongKey";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(4),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
