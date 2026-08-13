using CreditFlow.Web.Features.BandejaVerificacion.Models;

namespace CreditFlow.Web.Features.BandejaVerificacion.Services;

public interface IVerificacionService
{
    Task<List<VerificacionListItem>> ObtenerBandejaAsync(int? nCodAge = null);
}
