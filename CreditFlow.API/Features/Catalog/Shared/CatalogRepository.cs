using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Core.Errors;
using CreditFlow.API.Features.Catalog.Shared.Errors;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Catalog.Shared;

public class CatalogRepository(DbNegocioContext context) : ICatalogRepository
{
    public async Task<IReadOnlyList<CatalogoCodigo>> GetAllAsync() =>
        await context.CatalogoCodigos
            .AsNoTracking()
            .ToListAsync();

    public async Task<IReadOnlyList<CatalogoCodigo>> GetByCodeAsync(int code) =>
        await context.CatalogoCodigos
            .AsNoTracking()
            .Where(item => item.NCodigo == code)
            .OrderBy(item => item.NValor)
            .ToListAsync();

    public async Task AddAsync(CatalogoCodigo catalog)
    {
        var exists = await context.CatalogoCodigos.AnyAsync(item =>
            item.NCodigo == catalog.NCodigo &&
            item.NValor == catalog.NValor);

        if (exists)
        {
            throw new ResourceConflictException(
                CatalogErrors.AlreadyExists(catalog.NCodigo, catalog.NValor));
        }

        await context.CatalogoCodigos.AddAsync(catalog);
        await context.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(CatalogoCodigo catalog)
    {
        var existing = await context.CatalogoCodigos.FirstOrDefaultAsync(item =>
            item.NCodigo == catalog.NCodigo &&
            item.NValor == catalog.NValor);

        if (existing == null)
            return false;

        existing.CNomCod = catalog.CNomCod;
        existing.NEstados = catalog.NEstados;
        existing.NTipoCodigo = catalog.NTipoCodigo;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int code, int value)
    {
        var existing = await context.CatalogoCodigos.FirstOrDefaultAsync(item =>
            item.NCodigo == code &&
            item.NValor == value);

        if (existing == null)
            return false;

        context.CatalogoCodigos.Remove(existing);
        await context.SaveChangesAsync();
        return true;
    }
}
