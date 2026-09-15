using CreditFlow.API.Core.Errors;
using CreditFlow.API.Features.Credit.Shared.Errors;

namespace CreditFlow.API.Features.Credit.Web.ManageCreditLines;

public static class CreditLineRequestValidator
{
    public static void Validate(
        string description,
        int minimumTerm,
        int maximumTerm,
        decimal minimumAmount,
        decimal maximumAmount)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new RequestValidationException(CreditErrors.CreditLineDescriptionRequired);

        if (description.Trim().Length > 150)
            throw new RequestValidationException(CreditErrors.CreditLineDescriptionTooLong);

        if (minimumTerm > maximumTerm)
            throw new RequestValidationException(CreditErrors.InvalidCreditLineTermRange);

        if (minimumAmount > maximumAmount)
            throw new RequestValidationException(CreditErrors.InvalidCreditLineAmountRange);
    }
}
