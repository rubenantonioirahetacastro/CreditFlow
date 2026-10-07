namespace CreditFlow.API.Features.Authentication.Mobile.EmployeeLogin;

public sealed record EmployeeLoginResponse(
    bool Exito,
    string Mensaje,
    string Token,
    int? IdPersona,
    bool BTemporal,
    int IdRol,
    IReadOnlyList<int> IdRoles);
