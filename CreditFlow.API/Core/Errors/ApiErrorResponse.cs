namespace CreditFlow.API.Core.Errors;

public sealed record ApiErrorResponse(
    string Codigo,
    string Mensaje,
    IReadOnlyDictionary<string, string[]>? Errores = null);
