using CreditFlow.API.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Geography.Shared.GetMunicipalities;

[Route("api/Ubigeo/municipios")]
[ApiController]
public class GetMunicipalitiesController(DbNegocioContext context) : ControllerBase
{
    [HttpGet("{idDepartamento}")]
    public async Task<IActionResult> GetMunicipalities(int idDepartamento)
    {
        var municipalities = await context.Municipios
            .AsNoTracking()
            .Where(item => item.IdDepartamento == idDepartamento)
            .OrderBy(item => item.CNombre)
            .Select(item => new { item.IdMunicipio, item.CNombre, item.IdDepartamento })
            .ToListAsync();

        return Ok(municipalities);
    }

    [HttpGet("id/{id}", Name = "GetMunicipio")]
    public async Task<IActionResult> GetMunicipality(int id)
    {
        var municipality = await context.Municipios
            .AsNoTracking()
            .Where(item => item.IdMunicipio == id)
            .Select(item => new { item.IdMunicipio, item.CNombre, item.IdDepartamento })
            .FirstOrDefaultAsync();

        return municipality == null ? NotFound() : Ok(municipality);
    }
}
