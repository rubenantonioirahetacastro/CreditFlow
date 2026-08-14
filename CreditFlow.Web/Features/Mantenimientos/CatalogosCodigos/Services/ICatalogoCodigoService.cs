using CreditFlow.Web.Shared.CatalogoCodigos.Models;

namespace CreditFlow.Web.Services.Mantenimientos;

public interface ICatalogoCodigoService
{
    Task<List<CatalogoCodigoDto>> ObtenerTodosAsync();

    Task<(bool Exito, string? Mensaje)> CrearAsync(CatalogoCodigoDto catalogo);

    Task<(bool Exito, string? Mensaje)> ActualizarAsync(CatalogoCodigoDto catalogo);

    Task<(bool Exito, string? Mensaje)> EliminarAsync(int nCodigo, int nValor);
}
