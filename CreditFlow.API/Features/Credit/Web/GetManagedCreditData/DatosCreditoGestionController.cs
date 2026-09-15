using CreditFlow.API.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Credit.Web.GetManagedCreditData
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DatosCreditoGestionController : ControllerBase
    {
        private readonly DbNegocioContext _context;

        public DatosCreditoGestionController(DbNegocioContext context)
        {
            _context = context;
        }

        // GET api/DatosCreditoGestion/{nCodAge}/{nCodCred}
        [HttpGet("{nCodAge}/{nCodCred}")]
        public async Task<IActionResult> Obtener(int nCodAge, int nCodCred)
        {
            var credito = await _context.Creditos.AsNoTracking()
                .Where(c => c.NCodAge == nCodAge && c.NCodCred == nCodCred)
                .Select(c => new DatosCreditoGestionResponse
                {
                    NCodAge = c.NCodAge,
                    NCodCred = c.NCodCred,
                    NPrestamo = c.NPrestamo,
                    NEstado = c.NEstado
                })
                .FirstOrDefaultAsync();

            if (credito == null)
                return NotFound();

            return Ok(credito);
        }
    }
}
