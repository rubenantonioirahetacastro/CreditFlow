using CreditFlow.API.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Credit.Web.GetManagedPersonData
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DatosPersonaGestionController : ControllerBase
    {
        private readonly DbNegocioContext _context;

        public DatosPersonaGestionController(DbNegocioContext context)
        {
            _context = context;
        }

        // GET api/DatosPersonaGestion/{nCodAge}/{nCodCred}
        [HttpGet("{nCodAge}/{nCodCred}")]
        public async Task<IActionResult> Obtener(int nCodAge, int nCodCred)
        {
            var persona = await (
                from c in _context.Creditos.AsNoTracking()
                join p in _context.Personas.AsNoTracking() on c.IdPersona equals p.IdPersona
                where c.NCodAge == nCodAge && c.NCodCred == nCodCred
                select new DatosPersonaGestionResponse
                {
                    IdPersona = p.IdPersona,
                    CDocumento = p.CDocumento,
                    CNombres = p.CNombres,
                    CPrimerApellido = p.CPrimerApellido,
                    CSegundoApellido = p.CSegundoApellido,
                    CCorreo = p.CCorreo,
                    CTelefono = p.CTelefono,
                    CCelular = p.CCelular,
                    VFotoPerfil = p.VFotoPerfil,
                    CUsuarioGestion = p.CUsuarioGestion
                }
            ).FirstOrDefaultAsync();

            if (persona == null)
                return NotFound();

            return Ok(persona);
        }
    }
}
