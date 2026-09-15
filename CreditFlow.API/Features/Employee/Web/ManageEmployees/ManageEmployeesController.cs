using CreditFlow.API.Core.Security;
using CreditFlow.API.Core.Errors;
using CreditFlow.API.Features.Employee.Shared.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CreditFlow.API.Features.Employee.Web.ManageEmployees
{
    [Route("api/mantenimientos/empleados")]
    [ApiController]
    public class ManageEmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public ManageEmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        [Authorize(Policy = AuthorizationPolicies.Maintenance)]
        public async Task<IActionResult> ObtenerTodos()
        {
            var empleados = await _employeeService.GetAllAsync();
            return Ok(empleados);
        }

        [HttpGet("{id}/foto")]
        [Authorize(Policy = AuthorizationPolicies.Maintenance)]
        public async Task<IActionResult> ObtenerFoto(int id)
        {
            var foto = await _employeeService.GetPhotoAsync(id);
            if (foto == null)
                return NotFound();

            return File(foto.Value.Stream, foto.Value.ContentType);
        }

        [HttpPost]
        [Authorize(Policy = AuthorizationPolicies.Administration)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Crear([FromForm] CreateEmployeeRequest request)
        {
            var empleado = await _employeeService.CreateAsync(request);
            return Created($"api/mantenimientos/empleados/{empleado.IdEmpleado}", empleado);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = AuthorizationPolicies.Administration)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Actualizar(int id, [FromForm] UpdateEmployeeRequest request)
        {
            var empleado = await _employeeService.UpdateAsync(id, request);
            if (empleado == null)
                throw new ResourceNotFoundException(EmployeeErrors.NotFound);

            return Ok(empleado);
        }

        // Baja lógica: internamente solo desactiva (NEstado = 0) y bloquea el
        // UsuarioLogin asociado, no borra la fila. Mismo criterio que Roles.
        [HttpDelete("{id}")]
        [Authorize(Policy = AuthorizationPolicies.Administration)]
        public async Task<IActionResult> Eliminar(int id)
        {
            var eliminado = await _employeeService.DeleteAsync(id);
            if (!eliminado)
                throw new ResourceNotFoundException(EmployeeErrors.NotFound);

            return NoContent();
        }
    }
}
