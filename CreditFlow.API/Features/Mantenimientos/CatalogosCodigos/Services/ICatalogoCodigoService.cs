using CreditFlow.API.Domain.Entities;

namespace CreditFlow.API.Features.Mantenimientos.CatalogosCodigos.Services
{
    public interface ICatalogoCodigoService
    {
        Task<List<CatalogoCodigo>> AllCatalogos();
        Task<bool> AddCatalogo(CatalogoCodigo catalogo);
        Task<bool> UpdateCatalogo(CatalogoCodigo catalogo);
        Task<bool> DeleteCatalogo(int nCodigo, int nValor);
        Task<List<CatalogoCodigo>> GetCatalogoById(int nCodigo);
    }
}
