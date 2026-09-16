using System.ComponentModel.DataAnnotations;

namespace CreditFlow.Web.Core.Validation;

public static class EmailValidator
{
    private static readonly EmailAddressAttribute Attribute = new();

    public static bool IsValid(string? value) =>
        string.IsNullOrWhiteSpace(value) || Attribute.IsValid(value);
}
