using CreditFlow.API.Domain.Entities;

namespace CreditFlow.API.Features.Payment.Mobile.RegisterPayment;

public interface IRegisterPaymentHandler
{
    Task<List<CredCalendario>> ExecuteAsync(
        RegisterPaymentRequest request,
        CancellationToken cancellationToken = default);
}
