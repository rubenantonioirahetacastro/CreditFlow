using CreditFlow.Web.Core.Http;
using CreditFlow.Web.Shared.Catalog.Models;

namespace CreditFlow.Web.Features.Maintenance.Catalog.Services;

public interface ICatalogoCodigoService
{
    Task<ApiResult<List<CatalogoCodigoDto>>> ObtenerTodosAsync();

    Task<(bool Exito, string? Mensaje)> CrearAsync(CatalogoCodigoDto catalogo);

    Task<(bool Exito, string? Mensaje)> ActualizarAsync(CatalogoCodigoDto catalogo);

    Task<(bool Exito, string? Mensaje)> EliminarAsync(int nCodigo, int nValor);
}
