using CreditFlow.API.Features.ClientesMovil.Dto;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.ClientesMovil
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClientesMovilController : ControllerBase
    {
        private const int RolVerificador = 3; // Roles.IdRol

        private readonly DbNegocioContext _context;

        public ClientesMovilController(DbNegocioContext context)
        {
            _context = context;
        }

        // GET api/ClientesMovil (el rol sale del token, no de la app)
        [HttpGet]
        public async Task<IActionResult> Obtener()
        {
            if (!int.TryParse(User.FindFirst("IdRol")?.Value, out var idRol))
                return Unauthorized(new { Mensaje = "El token no trae un rol válido." });

            var estados = idRol switch
            {
                RolVerificador => new[] { 1, 2, 3 },
                _ => (int[]?)null
            };

            if (estados == null)
                return BadRequest(new { Mensaje = "Tu rol no tiene una bandeja móvil definida todavía." });

            var clientes = await (
                from c in _context.Creditos.AsNoTracking()
                where estados.Contains(c.NEstado)
                join p in _context.Personas.AsNoTracking() on c.IdPersona equals p.IdPersona into ps
                from p in ps.DefaultIfEmpty()
                join n in _context.Negocios.AsNoTracking() on c.IdNegocio equals n.IdNegocio into ns
                from n in ns.DefaultIfEmpty()
                select new ClienteMovilDto
                {
                    NCodAge = c.NCodAge,
                    NCodCred = c.NCodCred,
                    IdPersona = p == null ? (int?)null : p.IdPersona,
                    Nombre = p == null ? null : (p.CNombres + " " + p.CPrimerApellido + " " + (p.CSegundoApellido ?? "")).Trim(),
                    NombreNegocio = n == null ? null : n.CNombre,
                    Documento = p == null ? null : p.CDocumento,
                    Telefono = p == null ? null : p.CCelular,
                    DireccionNegocio = n == null ? null : n.CDireccion,
                    GeolocalizacionNegocio = n == null ? null : n.CGeolocalizacion
                }
            ).ToListAsync();

            return Ok(clientes);
        }
    }
}
