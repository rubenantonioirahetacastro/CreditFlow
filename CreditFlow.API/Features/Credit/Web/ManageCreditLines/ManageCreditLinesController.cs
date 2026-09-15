using CreditFlow.API.Core.Security;
using CreditFlow.API.Core.Errors;
using CreditFlow.API.Features.Credit.Shared.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CreditFlow.API.Features.Credit.Web.ManageCreditLines
{
    [Route("api/mantenimientos/lineas-credito")]
    [ApiController]
    public class ManageCreditLinesController : ControllerBase
    {
        private readonly ICreditLineManagementService _service;

        public ManageCreditLinesController(ICreditLineManagementService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Policy = AuthorizationPolicies.Maintenance)]
        public async Task<IActionResult> ObtenerTodas()
        {
            var lineas = await _service.GetAllAsync();
            return Ok(lineas);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = AuthorizationPolicies.Maintenance)]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var linea = await _service.GetByIdAsync(id);
            if (linea == null)
                throw new ResourceNotFoundException(CreditErrors.CreditLineNotFound);

            return Ok(linea);
        }

        [HttpPost]
        [Authorize(Policy = AuthorizationPolicies.Administration)]
        public async Task<IActionResult> Crear([FromBody] CreateCreditLineRequest request)
        {
            var linea = await _service.CreateAsync(request, GetCurrentUser());
            return Created($"api/mantenimientos/lineas-credito/{linea.NCodLinea}", linea);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = AuthorizationPolicies.Administration)]
        public async Task<IActionResult> Actualizar(int id, [FromBody] UpdateCreditLineRequest request)
        {
            var linea = await _service.UpdateAsync(id, request, GetCurrentUser());
            if (linea == null)
                throw new ResourceNotFoundException(CreditErrors.CreditLineNotFound);

            return Ok(linea);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = AuthorizationPolicies.Administration)]
        public async Task<IActionResult> Eliminar(int id)
        {
            var eliminado = await _service.DeleteAsync(id);
            if (!eliminado)
                throw new ResourceNotFoundException(CreditErrors.CreditLineNotFound);

            return NoContent();
        }

        private string? GetCurrentUser() =>
            User.Identity?.Name ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    }
}
