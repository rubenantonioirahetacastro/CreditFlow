using CreditFlow.API.Domain.Entities;

namespace CreditFlow.API.Features.Catalog.Shared;

public interface ICatalogRepository
{
    Task<IReadOnlyList<CatalogoCodigo>> GetAllAsync();
    Task<IReadOnlyList<CatalogoCodigo>> GetByCodeAsync(int code);
    Task AddAsync(CatalogoCodigo catalog);
    Task<bool> UpdateAsync(CatalogoCodigo catalog);
    Task<bool> DeleteAsync(int code, int value);
}
