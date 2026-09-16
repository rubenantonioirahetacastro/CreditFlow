using CreditFlow.Web.Core.Http;
using CreditFlow.Web.Features.Verification.Models;

namespace CreditFlow.Web.Features.Verification.Services;

public interface IVerificacionService
{
    Task<ApiResult<List<VerificacionListItem>>> ObtenerBandejaAsync(int? nCodAge = null);

    Task<string?> ObtenerFotoDataUrlAsync(int idPersona);
}
