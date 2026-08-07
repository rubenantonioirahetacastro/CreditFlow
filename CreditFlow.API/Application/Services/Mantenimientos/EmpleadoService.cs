using CreditFlow.API.Application.DTOs.Mantenimientos;
using CreditFlow.API.Application.Interfaces.Mantenimientos;
using CreditFlow.API.Application.Requests.Mantenimientos;
using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Infrastructure.Data;
using CreditFlow.API.Shared.Helpers;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Application.Services.Mantenimientos
{
    public class EmpleadoService : IEmpleadoService
    {
        private readonly DbNegocioContext _context;
        private readonly ErrorLogger _errorLogger;

        public EmpleadoService(DbNegocioContext context, ErrorLogger errorLogger)
        {
            _context = context;
            _errorLogger = errorLogger;
        }

        public async Task<List<EmpleadoDto>> ObtenerTodosAsync()
        {
            // Un empleado puede llegar a tener más de un UsuarioRole (asignado manualmente
            // fuera de CrearAsync). Se toma el rol asignado más recientemente -el vigente si
            // alguna vez se lo reasignaron- en vez de un JOIN, que devolvería una fila por rol.
            var empleados = await (
                from e in _context.Empleados.AsNoTracking()
                let ultimoRol = _context.UsuarioRoles.AsNoTracking()
                    .Where(ur => ur.IdUsuario == e.IdUsuario)
                    .OrderByDescending(ur => ur.FechaAsignacion)
                    .Select(ur => ur.IdRolNavigation.Nombre)
                    .FirstOrDefault()
                orderby e.CNombres
                select new EmpleadoDto
                {
                    IdEmpleado = e.IdEmpleado,
                    IdUsuario = e.IdUsuario,
                    Documento = e.CDocumento,
                    Nombres = e.CNombres,
                    PrimerApellido = e.CPrimerApellido,
                    SegundoApellido = e.CSegundoApellido,
                    Sexo = e.NSexo,
                    CodAgencia = e.NCodAge,
                    Correo = e.CCorreo,
                    Telefono = e.CTelefono,
                    Estado = e.NEstado,
                    Rol = ultimoRol
                }
            ).ToListAsync();

            return empleados;
        }

        public async Task<EmpleadoDto> CrearAsync(CrearEmpleadoRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Documento) || string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Documento y Password son obligatorios.");

            var documento = request.Documento.Trim();

            var existeUsuario = await _context.UsuarioLogins.AnyAsync(u => u.CDocumento == documento);
            if (existeUsuario)
                throw new InvalidOperationException($"Ya existe un usuario con el documento '{documento}'.");

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.IdRol == request.IdRol);
            if (role == null)
                throw new InvalidOperationException($"El rol con IdRol {request.IdRol} no existe.");

            await using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                // UsuarioLogin no conoce a Empleado: mismo patrón ya validado con Persona.
                // Se crea y guarda primero el login para poder asignar Empleado.IdUsuario.
                var usuario = new UsuarioLogin
                {
                    CDocumento = documento,
                    CCorreo = request.Correo,
                    Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    Token = null,
                    TokenTime = null,
                    TokenCheck = false,
                    Estado = 1,
                    IntentosFallidos = 0,
                    Bloqueado = 0,
                    UltimoLogin = null,
                    BContrasenaTemporal = false,
                    DFechaContrasenaTemporal = null
                };

                await _context.UsuarioLogins.AddAsync(usuario);
                // Save now to get IdUsuario assigned so Empleado and UsuarioRole can reference it
                await _context.SaveChangesAsync();

                var empleado = new Empleado
                {
                    IdUsuario = usuario.IdUsuario,
                    CDocumento = documento,
                    CNombres = request.Nombres,
                    CPrimerApellido = request.PrimerApellido,
                    CSegundoApellido = request.SegundoApellido,
                    NSexo = request.Sexo,
                    NCodAge = request.CodAgencia,
                    CCorreo = request.Correo,
                    CTelefono = request.Telefono,
                    NEstado = 1
                };

                await _context.Empleados.AddAsync(empleado);

                var usuarioRole = new UsuarioRole
                {
                    IdUsuario = usuario.IdUsuario,
                    IdRol = role.IdRol,
                    FechaAsignacion = DateTime.UtcNow
                };

                await _context.UsuarioRoles.AddAsync(usuarioRole);

                await _context.SaveChangesAsync();

                await tx.CommitAsync();

                return new EmpleadoDto
                {
                    IdEmpleado = empleado.IdEmpleado,
                    IdUsuario = usuario.IdUsuario,
                    Documento = empleado.CDocumento,
                    Nombres = empleado.CNombres,
                    PrimerApellido = empleado.CPrimerApellido,
                    SegundoApellido = empleado.CSegundoApellido,
                    Sexo = empleado.NSexo,
                    CodAgencia = empleado.NCodAge,
                    Correo = empleado.CCorreo,
                    Telefono = empleado.CTelefono,
                    Estado = empleado.NEstado,
                    Rol = role.Nombre
                };
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                await _errorLogger.LogAsync(ex);
                throw;
            }
        }
    }
}
