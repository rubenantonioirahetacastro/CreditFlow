namespace CreditFlow.API.Features.Verification.Web.GetCreditRequests;

public interface IObtenerBandejaVerificacionHandler
{
    Task<IReadOnlyList<BandejaVerificacionItemDto>> EjecutarAsync(int? nCodAge);
}
