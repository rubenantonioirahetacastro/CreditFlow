using CreditFlow.API.Core.Security;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Authentication.Shared.Session;

public sealed class UserSessionService(DbNegocioContext context) : IUserSessionService
{
    public async Task<UserSession> GetAsync(int userId, string document)
    {
        var assignedRoleIds = await context.UsuarioRoles
            .AsNoTracking()
            .Where(item => item.IdUsuario == userId && item.IdRolNavigation.Activo)
            .OrderByDescending(item => item.FechaAsignacion)
            .ThenByDescending(item => item.IdUsuarioRol)
            .Select(item => item.IdRol)
            .ToListAsync();

        var roleIds = assignedRoleIds.Distinct().ToList();
        var primaryRoleId = roleIds
            .FirstOrDefault(RoleIds.GlobalAdministrators.Contains);
        if (primaryRoleId == 0)
            primaryRoleId = roleIds.FirstOrDefault();

        if (primaryRoleId > 0)
        {
            roleIds.Remove(primaryRoleId);
            roleIds.Insert(0, primaryRoleId);
        }

        var personId = await context.Personas
            .AsNoTracking()
            .Where(item => item.IdUsuario == userId)
            .Select(item => (int?)item.IdPersona)
            .FirstOrDefaultAsync();

        var employeeId = await context.Empleados
            .AsNoTracking()
            .Where(item => item.IdUsuario == userId)
            .Select(item => (int?)item.IdEmpleado)
            .FirstOrDefaultAsync();

        return new UserSession(
            userId,
            document,
            personId,
            employeeId,
            primaryRoleId,
            roleIds);
    }
}
