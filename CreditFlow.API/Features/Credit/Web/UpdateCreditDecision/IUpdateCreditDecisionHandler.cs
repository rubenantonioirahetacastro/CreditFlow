namespace CreditFlow.API.Features.Credit.Web.UpdateCreditDecision;

public interface IUpdateCreditDecisionHandler
{
    Task ExecuteAsync(
        UpdateCreditDecisionRequest request,
        CancellationToken cancellationToken = default);
}
