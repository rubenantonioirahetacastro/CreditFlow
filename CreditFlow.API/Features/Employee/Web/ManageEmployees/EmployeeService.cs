using System.Globalization;
using System.Text;
using CreditFlow.API.Core.Diagnostics;
using CreditFlow.API.Core.Storage;
using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Core.Errors;
using CreditFlow.API.Features.Employee.Shared.Errors;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Employee.Web.ManageEmployees
{
    public class EmployeeService : IEmployeeService
    {
        private static readonly string[] ExtensionesFotoPermitidas = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long TamanoMaximoFotoBytes = 5 * 1024 * 1024; // 5MB

        private readonly DbNegocioContext _context;
        private readonly IErrorLogger _errorLogger;
        private readonly IBlobStorageService _blobStorageService;

        public EmployeeService(DbNegocioContext context, IErrorLogger errorLogger, IBlobStorageService blobStorageService)
        {
            _context = context;
            _errorLogger = errorLogger;
            _blobStorageService = blobStorageService;
        }

        public async Task<List<EmployeeDto>> GetAllAsync()
        {
            // Un empleado puede llegar a tener más de un UsuarioRole (asignado manualmente
            // fuera de CrearAsync). Se toma el rol asignado más recientemente -el vigente si
            // alguna vez se lo reasignaron- en vez de un JOIN, que devolvería una fila por rol.
            var empleados = await (
                from e in _context.Empleados.AsNoTracking()
                let ultimoRol = _context.UsuarioRoles.AsNoTracking()
                    .Where(ur => ur.IdUsuario == e.IdUsuario)
                    .OrderByDescending(ur => ur.FechaAsignacion)
                    .Select(ur => new { ur.IdRol, ur.IdRolNavigation.Nombre })
                    .FirstOrDefault()
                let codigoUsuario = _context.UsuarioLogins.AsNoTracking()
                    .Where(u => u.IdUsuario == e.IdUsuario)
                    .Select(u => u.CCodUsu)
                    .FirstOrDefault()
                let fotoUrl = _context.UsuarioLogins.AsNoTracking()
                    .Where(u => u.IdUsuario == e.IdUsuario)
                    .Select(u => u.VFoto)
                    .FirstOrDefault()
                orderby e.CNombres
                select new EmployeeDto
                {
                    IdEmpleado = e.IdEmpleado,
                    IdUsuario = e.IdUsuario,
                    Documento = e.CDocumento,
                    CodigoUsuario = codigoUsuario ?? string.Empty,
                    FotoUrl = fotoUrl,
                    Nombres = e.CNombres,
                    PrimerApellido = e.CPrimerApellido,
                    SegundoApellido = e.CSegundoApellido,
                    Sexo = e.NSexo,
                    CodAgencia = e.NCodAge,
                    Correo = e.CCorreo,
                    Telefono = e.CTelefono,
                    Estado = e.NEstado,
                    IdRol = ultimoRol == null ? 0 : ultimoRol.IdRol,
                    Rol = ultimoRol == null ? null : ultimoRol.Nombre
                }
            ).ToListAsync();

            return empleados;
        }

        public async Task<EmployeeDto> CreateAsync(CreateEmployeeRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Documento) || string.IsNullOrWhiteSpace(request.Password))
                throw new RequestValidationException(EmployeeErrors.CredentialsRequired);

            var documento = request.Documento.Trim();

            var existeUsuario = await _context.UsuarioLogins.AnyAsync(u => u.CDocumento == documento);
            if (existeUsuario)
                throw new ResourceConflictException(EmployeeErrors.UserAlreadyExists(documento));

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.IdRol == request.IdRol);
            if (role == null)
                throw new ResourceNotFoundException(EmployeeErrors.RoleNotFound(request.IdRol));

            await using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                var codigoUsuario = await GenerarCodigoUsuarioAsync(request.Nombres, request.PrimerApellido, documento);
                var fotoUrl = await SubirFotoAsync(request.Foto, documento);

                // UsuarioLogin no conoce a Empleado: mismo patrón ya validado con Persona.
                // Se crea y guarda primero el login para poder asignar Empleado.IdUsuario.
                var usuario = new UsuarioLogin
                {
                    CDocumento = documento,
                    CCodUsu = codigoUsuario,
                    VFoto = fotoUrl,
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

                return new EmployeeDto
                {
                    IdEmpleado = empleado.IdEmpleado,
                    IdUsuario = usuario.IdUsuario,
                    Documento = empleado.CDocumento,
                    CodigoUsuario = usuario.CCodUsu,
                    FotoUrl = usuario.VFoto,
                    Nombres = empleado.CNombres,
                    PrimerApellido = empleado.CPrimerApellido,
                    SegundoApellido = empleado.CSegundoApellido,
                    Sexo = empleado.NSexo,
                    CodAgencia = empleado.NCodAge,
                    Correo = empleado.CCorreo,
                    Telefono = empleado.CTelefono,
                    Estado = empleado.NEstado,
                    IdRol = role.IdRol,
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

        public async Task<EmployeeDto?> UpdateAsync(int id, UpdateEmployeeRequest request)
        {
            var empleado = await _context.Empleados.FirstOrDefaultAsync(e => e.IdEmpleado == id);
            if (empleado == null)
                return null;

            empleado.CNombres = request.Nombres;
            empleado.CPrimerApellido = request.PrimerApellido;
            empleado.CSegundoApellido = request.SegundoApellido;
            empleado.NSexo = request.Sexo;
            empleado.NCodAge = request.CodAgencia;
            empleado.CCorreo = request.Correo;
            empleado.CTelefono = request.Telefono;
            var cambioEstado = empleado.NEstado != request.Estado;
            empleado.NEstado = request.Estado;

            // Empleado.CCorreo y UsuarioLogin.CCorreo se cargan con el mismo valor en
            // CrearAsync; se mantienen sincronizados también al editar.
            var usuario = await _context.UsuarioLogins.FirstOrDefaultAsync(u => u.IdUsuario == empleado.IdUsuario);
            if (usuario != null)
            {
                usuario.CCorreo = request.Correo;

                // Mismo criterio que DeleteAsync: el login solo revisa Bloqueado, así que
                // desactivar bloquea el acceso y reactivar lo libera. Solo cuando el estado
                // cambia, para no desbloquear por intentos fallidos en una edición cualquiera.
                if (cambioEstado)
                {
                    var activo = request.Estado == 1;
                    usuario.Bloqueado = activo ? 0 : 1;
                    usuario.FechaBloqueo = activo ? null : int.Parse(DateTime.UtcNow.ToString("yyyyMMdd"));
                    if (activo)
                        usuario.IntentosFallidos = 0;
                }

                if (request.Foto != null)
                    usuario.VFoto = await SubirFotoAsync(request.Foto, empleado.CDocumento);
            }

            var idRolActual = await ObtenerIdRolActualAsync(empleado.IdUsuario);
            if (idRolActual != request.IdRol)
            {
                var nuevoRol = await _context.Roles.FirstOrDefaultAsync(r => r.IdRol == request.IdRol);
                if (nuevoRol == null)
                    throw new ResourceNotFoundException(EmployeeErrors.RoleNotFound(request.IdRol));

                // No se reemplaza la fila anterior de UsuarioRoles: se agrega una nueva y
                // ObtenerTodosAsync ya resuelve el rol vigente como el más recientemente
                // asignado, preservando el historial de reasignaciones.
                await _context.UsuarioRoles.AddAsync(new UsuarioRole
                {
                    IdUsuario = empleado.IdUsuario,
                    IdRol = request.IdRol,
                    FechaAsignacion = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();

            var rolVigente = await _context.Roles
                .Where(r => r.IdRol == request.IdRol)
                .Select(r => r.Nombre)
                .FirstOrDefaultAsync();

            return new EmployeeDto
            {
                IdEmpleado = empleado.IdEmpleado,
                IdUsuario = empleado.IdUsuario,
                Documento = empleado.CDocumento,
                CodigoUsuario = usuario?.CCodUsu ?? string.Empty,
                FotoUrl = usuario?.VFoto,
                Nombres = empleado.CNombres,
                PrimerApellido = empleado.CPrimerApellido,
                SegundoApellido = empleado.CSegundoApellido,
                Sexo = empleado.NSexo,
                CodAgencia = empleado.NCodAge,
                Correo = empleado.CCorreo,
                Telefono = empleado.CTelefono,
                Estado = empleado.NEstado,
                IdRol = request.IdRol,
                Rol = rolVigente
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var empleado = await _context.Empleados.FirstOrDefaultAsync(e => e.IdEmpleado == id);
            if (empleado == null)
                return false;

            // Baja lógica: no se borra la fila (preserva historial e integridad
            // referencial). Se bloquea también el UsuarioLogin asociado para que un
            // empleado dado de baja no pueda seguir iniciando sesión.
            empleado.NEstado = 0;

            var usuario = await _context.UsuarioLogins.FirstOrDefaultAsync(u => u.IdUsuario == empleado.IdUsuario);
            if (usuario != null)
            {
                usuario.Bloqueado = 1;
                usuario.FechaBloqueo = int.Parse(DateTime.UtcNow.ToString("yyyyMMdd"));
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(Stream Stream, string ContentType)?> GetPhotoAsync(int id)
        {
            var fotoPath = await _context.Empleados.AsNoTracking()
                .Where(e => e.IdEmpleado == id)
                .Join(_context.UsuarioLogins.AsNoTracking(), e => e.IdUsuario, u => u.IdUsuario, (e, u) => u.VFoto)
                .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(fotoPath))
                return null;

            var stream = await _blobStorageService.DownloadImageAsync(fotoPath);
            if (stream == null)
                return null;

            return (stream, ObtenerContentType(Path.GetExtension(fotoPath)));
        }

        private static string ObtenerContentType(string extension) => extension.ToLowerInvariant() switch
        {
            ".png" => "image/png",
            ".webp" => "image/webp",
            _ => "image/jpeg"
        };

        private async Task<int?> ObtenerIdRolActualAsync(int idUsuario)
        {
            return await _context.UsuarioRoles
                .Where(ur => ur.IdUsuario == idUsuario)
                .OrderByDescending(ur => ur.FechaAsignacion)
                .Select(ur => (int?)ur.IdRol)
                .FirstOrDefaultAsync();
        }

        private async Task<string?> SubirFotoAsync(IFormFile? foto, string documento)
        {
            if (foto == null || foto.Length == 0)
                return null;

            var extension = Path.GetExtension(foto.FileName).ToLowerInvariant();
            if (!ExtensionesFotoPermitidas.Contains(extension))
                throw new RequestValidationException(
                    EmployeeErrors.InvalidPhotoType(extension, ExtensionesFotoPermitidas));

            if (foto.Length > TamanoMaximoFotoBytes)
                throw new RequestValidationException(
                    EmployeeErrors.PhotoTooLarge(TamanoMaximoFotoBytes / (1024 * 1024)));

            var fileName = $"{Guid.NewGuid()}{extension}";
            await using var stream = foto.OpenReadStream();
            return await _blobStorageService.UploadImageAsync(stream, $"empleados/{documento}", fileName);
        }

        // Código de usuario: inicial del nombre + primer apellido completo + últimos 2
        // dígitos del documento, en mayúsculas y sin tildes/espacios. Si ya existe, se
        // agrega un correlativo (_1, _2, ...) hasta encontrar uno libre.
        private async Task<string> GenerarCodigoUsuarioAsync(string nombres, string primerApellido, string documento)
        {
            var inicialNombre = NormalizarTexto(nombres).FirstOrDefault();
            var apellido = NormalizarTexto(primerApellido);
            var digitosDocumento = new string(documento.Where(char.IsDigit).ToArray());
            var ultimosDigitos = digitosDocumento.Length >= 2
                ? digitosDocumento[^2..]
                : digitosDocumento;

            var baseCodigo = $"{inicialNombre}{apellido}{ultimosDigitos}";

            var codigo = baseCodigo;
            var sufijo = 1;
            while (await _context.UsuarioLogins.AnyAsync(u => u.CCodUsu == codigo))
            {
                codigo = $"{baseCodigo}_{sufijo}";
                sufijo++;
            }

            return codigo;
        }

        private static string NormalizarTexto(string texto)
        {
            var primeraPalabra = texto.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? string.Empty;

            var formaDescompuesta = primeraPalabra.Normalize(NormalizationForm.FormD);
            var sinTildes = new StringBuilder();
            foreach (var c in formaDescompuesta)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sinTildes.Append(c);
            }

            return sinTildes.ToString().Normalize(NormalizationForm.FormC).ToUpperInvariant();
        }
    }
}
