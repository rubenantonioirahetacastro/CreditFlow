namespace CreditFlow.Web.Core.Validation;

public readonly record struct UiValidationResult(bool IsValid, string Message)
{
    public static UiValidationResult Success() => new(true, string.Empty);

    public static UiValidationResult Failure(string message) => new(false, message);
}
