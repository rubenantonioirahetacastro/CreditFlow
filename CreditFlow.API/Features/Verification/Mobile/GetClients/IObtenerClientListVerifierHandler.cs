namespace CreditFlow.API.Features.Verification.Mobile.GetClients;

public interface IObtenerClientListVerifierHandler
{
    Task<IReadOnlyList<ClientListVerifierDto>> EjecutarAsync();
}
