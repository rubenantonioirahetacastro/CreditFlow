namespace CreditFlow.API.Features.Authentication.Shared.GetUserProfile;

public sealed record UserProfileResponse(
    int IdUsuario,
    string Tipo,
    int? IdPersona,
    int? IdEmpleado,
    string Nombres,
    string Documento,
    string? Correo,
    string? Telefono,
    string? Celular,
    string? FotoUrl);
