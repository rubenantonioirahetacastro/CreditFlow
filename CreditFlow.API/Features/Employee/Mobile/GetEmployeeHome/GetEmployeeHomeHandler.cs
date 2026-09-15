using CreditFlow.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CreditFlow.API.Features.Employee.Mobile.GetEmployeeHome;

public sealed class GetEmployeeHomeHandler(
    DbNegocioContext context,
    IOptions<EmployeeHomeOptions> options) : IGetEmployeeHomeHandler
{
    public async Task<EmployeeHomeDto?> ExecuteAsync(int employeeId, int roleId)
    {
        var employee = await context.Empleados
            .AsNoTracking()
            .Where(item => item.IdEmpleado == employeeId)
            .Select(item => new
            {
                item.IdEmpleado,
                item.CNombres,
                item.CPrimerApellido,
                item.CSegundoApellido,
                item.NCodAge
            })
            .FirstOrDefaultAsync();

        if (employee == null)
            return null;

        var roleName = await context.Roles
            .AsNoTracking()
            .Where(role => role.IdRol == roleId)
            .Select(role => role.Nombre)
            .FirstOrDefaultAsync() ?? roleId.ToString();

        var (startUtc, endUtc) = GetCurrentLocalDayUtcRange();
        var completedToday = await context.VerificacionCreditos
            .AsNoTracking()
            .CountAsync(item =>
                item.IdEmpleado == employee.IdEmpleado &&
                item.DFecha >= startUtc &&
                item.DFecha < endUtc);

        var fullName = string.Join(
            " ",
            new[]
            {
                employee.CNombres,
                employee.CPrimerApellido,
                employee.CSegundoApellido
            }.Where(value => !string.IsNullOrWhiteSpace(value)));

        return new EmployeeHomeDto(
            new EmployeeSummaryDto(
                employee.IdEmpleado,
                fullName,
                roleId,
                roleName,
                employee.NCodAge),
            new EmployeeProgressDto(
                completedToday,
                options.Value.VerifierDailyGoal));
    }

    private (DateTime StartUtc, DateTime EndUtc) GetCurrentLocalDayUtcRange()
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(options.Value.TimeZoneId);
        var localDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone).Date;
        var startUtc = TimeZoneInfo.ConvertTimeToUtc(localDate, timeZone);
        var endUtc = TimeZoneInfo.ConvertTimeToUtc(localDate.AddDays(1), timeZone);
        return (startUtc, endUtc);
    }
}
