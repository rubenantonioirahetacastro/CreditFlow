using CreditFlow.API.Core.Errors;
using CreditFlow.API.Features.Credit.Shared.Errors;

namespace CreditFlow.API.Features.Credit.Web.UpdateCreditDecision;

public static class UpdateCreditDecisionValidator
{
    public const int StatusUnderAnalysis = 2;
    public const int StatusApproved = 3;
    public const int StatusCancelled = 50;

    private static readonly HashSet<int> AllowedStatuses =
    [
        StatusUnderAnalysis,
        StatusApproved,
        StatusCancelled,
    ];

    public static void Validate(UpdateCreditDecisionRequest request)
    {
        if (!AllowedStatuses.Contains(request.NEstado))
            throw new RequestValidationException(CreditErrors.InvalidDecisionStatus);
    }
}
