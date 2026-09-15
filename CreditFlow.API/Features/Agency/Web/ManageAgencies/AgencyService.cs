using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Core.Errors;
using CreditFlow.API.Features.Agency.Shared.Errors;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Agency.Web.ManageAgencies;

public class AgencyService(DbNegocioContext context) : IAgencyService
{
    public async Task<IReadOnlyList<AgencyDto>> GetAllAsync() =>
        await context.Agencias
            .AsNoTracking()
            .OrderBy(item => item.CNomAge)
            .Select(item => new AgencyDto
            {
                NCodAge = item.NCodAge,
                Nombre = item.CNomAge,
                Direccion = item.CDirecAge,
                Telefono = item.CTelefAge,
                CorreoElectronico = item.CCorreoElectronico
            })
            .ToListAsync();

    public async Task<AgencyDto?> GetByIdAsync(int id)
    {
        var agency = await context.Agencias
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.NCodAge == id);

        return agency == null ? null : Map(agency);
    }

    public async Task<AgencyDto> CreateAsync(CreateAgencyRequest request)
    {
        var exists = await context.Agencias.AnyAsync(item => item.NCodAge == request.NCodAge);
        if (exists)
            throw new ResourceConflictException(AgencyErrors.AlreadyExists(request.NCodAge));

        var agency = new Agencia
        {
            NCodAge = request.NCodAge,
            CNomAge = request.Nombre,
            CDirecAge = request.Direccion,
            CTelefAge = request.Telefono,
            CCorreoElectronico = request.CorreoElectronico
        };

        await context.Agencias.AddAsync(agency);
        await context.SaveChangesAsync();

        return Map(agency);
    }

    public async Task<AgencyDto?> UpdateAsync(int id, UpdateAgencyRequest request)
    {
        var agency = await context.Agencias.FirstOrDefaultAsync(item => item.NCodAge == id);
        if (agency == null)
            return null;

        agency.CNomAge = request.Nombre;
        agency.CDirecAge = request.Direccion;
        agency.CTelefAge = request.Telefono;
        agency.CCorreoElectronico = request.CorreoElectronico;

        await context.SaveChangesAsync();
        return Map(agency);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var agency = await context.Agencias.FirstOrDefaultAsync(item => item.NCodAge == id);
        if (agency == null)
            return false;

        context.Agencias.Remove(agency);
        await context.SaveChangesAsync();
        return true;
    }

    private static AgencyDto Map(Agencia agency) => new()
    {
        NCodAge = agency.NCodAge,
        Nombre = agency.CNomAge,
        Direccion = agency.CDirecAge,
        Telefono = agency.CTelefAge,
        CorreoElectronico = agency.CCorreoElectronico
    };
}
