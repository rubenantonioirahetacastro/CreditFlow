using CreditFlow.API.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Geography.Shared.GetDepartments;

[Route("api/Ubigeo/departamentos")]
[ApiController]
public class GetDepartmentsController(DbNegocioContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetDepartments()
    {
        var departments = await context.Departamentos
            .AsNoTracking()
            .OrderBy(item => item.CNombre)
            .Select(item => new { item.IdDepartamento, item.CNombre })
            .ToListAsync();

        return Ok(departments);
    }

    [HttpGet("{id}", Name = "GetDepartamento")]
    public async Task<IActionResult> GetDepartment(int id)
    {
        var department = await context.Departamentos
            .AsNoTracking()
            .Where(item => item.IdDepartamento == id)
            .Select(item => new { item.IdDepartamento, item.CNombre })
            .FirstOrDefaultAsync();

        return department == null ? NotFound() : Ok(department);
    }
}
