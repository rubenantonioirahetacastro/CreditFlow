using CreditFlow.API.Core.Security;
using CreditFlow.API.Core.Errors;
using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Features.Catalog.Shared;
using CreditFlow.API.Features.Catalog.Shared.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CreditFlow.API.Features.Catalog.Web.ManageCatalogs;

[Route("api/CatalogoCodigo")]
[ApiController]
[Authorize(Policy = AuthorizationPolicies.Maintenance)]
public class ManageCatalogsController(ICatalogRepository repository) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddCatalog([FromBody] CatalogRequest request)
    {
        var catalog = Map(request);

        await repository.AddAsync(catalog);
        return CreatedAtRoute(
            "GetCatalogByCode",
            new { codigo = catalog.NCodigo },
            catalog);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateCatalog([FromBody] CatalogRequest request)
    {
        var updated = await repository.UpdateAsync(Map(request));
        if (!updated)
            throw new ResourceNotFoundException(CatalogErrors.NotFound);

        return NoContent();
    }

    [HttpDelete("{codigo}/{valor}")]
    public async Task<IActionResult> DeleteCatalog(int codigo, int valor)
    {
        var deleted = await repository.DeleteAsync(codigo, valor);
        if (!deleted)
            throw new ResourceNotFoundException(CatalogErrors.NotFound);

        return NoContent();
    }

    private static CatalogoCodigo Map(CatalogRequest request) => new()
    {
        NCodigo = request.NCodigo,
        NValor = request.NValor,
        CNomCod = request.CNomCod,
        NEstados = request.NEstados,
        NTipoCodigo = request.NTipoCodigo
    };
}
