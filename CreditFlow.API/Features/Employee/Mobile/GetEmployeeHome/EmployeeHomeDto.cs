namespace CreditFlow.API.Features.Employee.Mobile.GetEmployeeHome;

public sealed record EmployeeHomeDto(
    EmployeeSummaryDto Empleado,
    EmployeeProgressDto Progreso);

public sealed record EmployeeSummaryDto(
    int Id,
    string NombreCompleto,
    int IdRol,
    string NombreRol,
    int CodigoAgencia);

public sealed record EmployeeProgressDto(
    int CompletadosHoy,
    int MetaDiaria);
