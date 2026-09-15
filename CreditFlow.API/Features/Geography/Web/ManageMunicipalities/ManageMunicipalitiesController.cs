using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace CreditFlow.API.Features.Geography.Web.ManageMunicipalities;

[Route("api/Ubigeo/municipios")]
[ApiController]
public class ManageMunicipalitiesController(DbNegocioContext context) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateMunicipality([FromBody] MunicipalityRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.CNombre))
            return BadRequest("Municipio inválido");

        var department = await context.Departamentos.FindAsync(request.IdDepartamento);
        if (department == null)
            return BadRequest("Departamento no existe");

        var municipality = new Municipio
        {
            IdMunicipio = request.IdMunicipio,
            IdDepartamento = request.IdDepartamento,
            CNombre = request.CNombre
        };

        context.Municipios.Add(municipality);
        await context.SaveChangesAsync();

        return CreatedAtRoute(
            "GetMunicipio",
            new { id = municipality.IdMunicipio },
            municipality);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMunicipality(int id, [FromBody] MunicipalityRequest request)
    {
        if (request == null || id != request.IdMunicipio)
            return BadRequest();

        var existing = await context.Municipios.FindAsync(id);
        if (existing == null)
            return NotFound();

        var department = await context.Departamentos.FindAsync(request.IdDepartamento);
        if (department == null)
            return BadRequest("Departamento no existe");

        existing.CNombre = request.CNombre;
        existing.IdDepartamento = request.IdDepartamento;

        context.Municipios.Update(existing);
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMunicipality(int id)
    {
        var existing = await context.Municipios.FindAsync(id);
        if (existing == null)
            return NotFound();

        context.Municipios.Remove(existing);
        await context.SaveChangesAsync();

        return NoContent();
    }
}
