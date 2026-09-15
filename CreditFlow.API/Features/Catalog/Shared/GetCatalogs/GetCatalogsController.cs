using Microsoft.AspNetCore.Mvc;

namespace CreditFlow.API.Features.Catalog.Shared.GetCatalogs;

[Route("api/CatalogoCodigo")]
[ApiController]
public class GetCatalogsController(ICatalogRepository repository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllCatalogs() =>
        Ok(await repository.GetAllAsync());

    [HttpGet("{codigo}", Name = "GetCatalogByCode")]
    public async Task<IActionResult> GetCatalogByCode(int codigo)
    {
        var catalogs = await repository.GetByCodeAsync(codigo);

        return catalogs.Count == 0
            ? NotFound(new { Mensaje = "No existen valores para este catálogo." })
            : Ok(catalogs);
    }
}
