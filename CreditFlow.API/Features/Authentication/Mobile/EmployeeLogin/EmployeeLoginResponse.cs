namespace CreditFlow.API.Features.Authentication.Mobile.EmployeeLogin;

public sealed record EmployeeLoginResponse(
    bool Exito,
    string Mensaje,
    string Token,
    int IdUsuario,
    bool BTemporal,
    IReadOnlyList<string> Capacidades);
