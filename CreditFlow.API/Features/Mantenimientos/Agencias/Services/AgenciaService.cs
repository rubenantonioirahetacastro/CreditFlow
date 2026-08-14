using CreditFlow.API.Features.Mantenimientos.Agencias.DTOs;
using CreditFlow.API.Features.Mantenimientos.Agencias.Services;
using CreditFlow.API.Features.Mantenimientos.Agencias.Requests;
using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Mantenimientos.Agencias.Services
{
    public class AgenciaService : IAgenciaService
    {
        private readonly DbNegocioContext _context;

        public AgenciaService(DbNegocioContext context)
        {
            _context = context;
        }

        public async Task<List<AgenciaDto>> ObtenerTodasAsync()
        {
            return await _context.Agencias
                .AsNoTracking()
                .OrderBy(a => a.CNomAge)
                .Select(a => new AgenciaDto
                {
                    NCodAge = a.NCodAge,
                    Nombre = a.CNomAge,
                    Direccion = a.CDirecAge,
                    Telefono = a.CTelefAge,
                    CorreoElectronico = a.CCorreoElectronico
                })
                .ToListAsync();
        }

        public async Task<AgenciaDto?> ObtenerPorIdAsync(int id)
        {
            var agencia = await _context.Agencias.AsNoTracking().FirstOrDefaultAsync(a => a.NCodAge == id);
            if (agencia == null)
                return null;

            return new AgenciaDto
            {
                NCodAge = agencia.NCodAge,
                Nombre = agencia.CNomAge,
                Direccion = agencia.CDirecAge,
                Telefono = agencia.CTelefAge,
                CorreoElectronico = agencia.CCorreoElectronico
            };
        }

        public async Task<AgenciaDto> CrearAsync(CrearAgenciaRequest request)
        {
            var existe = await _context.Agencias.AnyAsync(a => a.NCodAge == request.NCodAge);
            if (existe)
                throw new InvalidOperationException($"Ya existe una agencia con el código {request.NCodAge}.");

            var agencia = new Agencia
            {
                NCodAge = request.NCodAge,
                CNomAge = request.Nombre,
                CDirecAge = request.Direccion,
                CTelefAge = request.Telefono,
                CCorreoElectronico = request.CorreoElectronico
            };

            await _context.Agencias.AddAsync(agencia);
            await _context.SaveChangesAsync();

            return new AgenciaDto
            {
                NCodAge = agencia.NCodAge,
                Nombre = agencia.CNomAge,
                Direccion = agencia.CDirecAge,
                Telefono = agencia.CTelefAge,
                CorreoElectronico = agencia.CCorreoElectronico
            };
        }

        public async Task<AgenciaDto?> ActualizarAsync(int id, ActualizarAgenciaRequest request)
        {
            var agencia = await _context.Agencias.FirstOrDefaultAsync(a => a.NCodAge == id);
            if (agencia == null)
                return null;

            agencia.CNomAge = request.Nombre;
            agencia.CDirecAge = request.Direccion;
            agencia.CTelefAge = request.Telefono;
            agencia.CCorreoElectronico = request.CorreoElectronico;

            await _context.SaveChangesAsync();

            return new AgenciaDto
            {
                NCodAge = agencia.NCodAge,
                Nombre = agencia.CNomAge,
                Direccion = agencia.CDirecAge,
                Telefono = agencia.CTelefAge,
                CorreoElectronico = agencia.CCorreoElectronico
            };
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var agencia = await _context.Agencias.FirstOrDefaultAsync(a => a.NCodAge == id);
            if (agencia == null)
                return false;

            _context.Agencias.Remove(agencia);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
