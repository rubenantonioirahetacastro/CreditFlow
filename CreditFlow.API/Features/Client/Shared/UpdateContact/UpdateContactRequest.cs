namespace CreditFlow.API.Features.Client.Shared.UpdateContact;

public sealed record UpdateContactRequest(
    string? CCorreo,
    string CTelefono,
    string CCelular);
