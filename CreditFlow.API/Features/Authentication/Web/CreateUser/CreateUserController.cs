using CreditFlow.API.Core.Security;
using CreditFlow.API.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Authentication.Web.CreateUser;

[ApiController]
[Route("api/Token")]
public sealed class CreateUserController(DbNegocioContext context) : ControllerBase
{
    [HttpPost("create-user")]
    [Authorize(Policy = AuthorizationPolicies.Administration)]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Documento) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Documento y Password son obligatorios.");
        }

        var document = request.Documento.Trim();
        var existingUser = await context.UsuarioLogins
            .FirstOrDefaultAsync(item => item.CDocumento == document);

        if (existingUser != null)
            return Conflict("Usuario ya existe.");

        var roleExists = await context.Roles
            .AnyAsync(item => item.IdRol == request.IdRol && item.Activo);
        if (!roleExists)
            return BadRequest(new { Mensaje = $"El rol {request.IdRol} no existe o está inactivo." });

        var user = new UsuarioLogin
        {
            CDocumento = document,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CCorreo = request.Correo ?? string.Empty,
            Token = null,
            TokenTime = null,
            TokenCheck = false,
            Estado = 1,
            IntentosFallidos = 0,
            Bloqueado = 0,
            UltimoLogin = null,
            BContrasenaTemporal = false,
            DFechaContrasenaTemporal = null
        };

        await context.UsuarioLogins.AddAsync(user);
        await context.SaveChangesAsync();

        await context.UsuarioRoles.AddAsync(new UsuarioRole
        {
            IdUsuario = user.IdUsuario,
            IdRol = request.IdRol,
            FechaAsignacion = DateTime.UtcNow
        });

        await context.SaveChangesAsync();
        return Ok(new { Mensaje = "Usuario creado" });
    }
}
