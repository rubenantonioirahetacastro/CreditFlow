using CreditFlow.Web.Core.Utils.Format;
using CreditFlow.Web.Core.Validation;

namespace CreditFlow.Web.Shared.Identity.Validation;

public static class DocumentValidator
{
    public static UiValidationResult Validate(string? value, DocumentType type)
    {
        var digits = DocumentFormatter.Normalize(value, type);
        var expectedLength = type == DocumentType.Dui ? 9 : 14;

        return digits.Length == expectedLength
            ? UiValidationResult.Success()
            : UiValidationResult.Failure(
                type == DocumentType.Dui
                    ? "El DUI debe contener 9 dígitos."
                    : "El NIT debe contener 14 dígitos.");
    }
}
