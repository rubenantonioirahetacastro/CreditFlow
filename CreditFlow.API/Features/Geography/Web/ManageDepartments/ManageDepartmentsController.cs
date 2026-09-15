using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Geography.Web.ManageDepartments;

[Route("api/Ubigeo/departamentos")]
[ApiController]
public class ManageDepartmentsController(DbNegocioContext context) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateDepartment([FromBody] DepartmentRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.CNombre))
            return BadRequest("Departamento inválido");

        var department = new Departamento
        {
            IdDepartamento = request.IdDepartamento,
            CNombre = request.CNombre
        };

        context.Departamentos.Add(department);
        await context.SaveChangesAsync();

        return CreatedAtRoute(
            "GetDepartamento",
            new { id = department.IdDepartamento },
            department);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDepartment(int id, [FromBody] DepartmentRequest request)
    {
        if (request == null || id != request.IdDepartamento)
            return BadRequest();

        var existing = await context.Departamentos.FindAsync(id);
        if (existing == null)
            return NotFound();

        existing.CNombre = request.CNombre;

        context.Departamentos.Update(existing);
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        var existing = await context.Departamentos
            .Include(item => item.Municipios)
            .FirstOrDefaultAsync(item => item.IdDepartamento == id);
        if (existing == null)
            return NotFound();

        context.Departamentos.Remove(existing);
        await context.SaveChangesAsync();

        return NoContent();
    }
}
