using CreditFlow.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Verification.Mobile.GetClients;

public sealed class ObtenerClientListVerifierHandler(
    DbNegocioContext context) : IObtenerClientListVerifierHandler
{
    private const int EstadoPendienteDeVerificacion = 2;

    public async Task<IReadOnlyList<ClientListVerifierDto>> EjecutarAsync()
    {
        return await (
            from credito in context.Creditos.AsNoTracking()
            where credito.NEstado == EstadoPendienteDeVerificacion
            join persona in context.Personas.AsNoTracking()
                on credito.IdPersona equals persona.IdPersona into personas
            from persona in personas.DefaultIfEmpty()
            join negocio in context.Negocios.AsNoTracking()
                on credito.IdNegocio equals negocio.IdNegocio into negocios
            from negocio in negocios.DefaultIfEmpty()
            select new ClientListVerifierDto
            {
                NCodAge = credito.NCodAge,
                NCodCred = credito.NCodCred,
                IdPersona = persona == null ? null : persona.IdPersona,
                Nombre = persona == null
                    ? null
                    : (persona.CNombres + " " + persona.CPrimerApellido + " " +
                        (persona.CSegundoApellido ?? "")).Trim(),
                NombreNegocio = negocio == null ? null : negocio.CNombre,
                Documento = persona == null ? null : persona.CDocumento,
                Telefono = persona == null ? null : persona.CCelular,
                DireccionNegocio = negocio == null ? null : negocio.CDireccion,
                GeolocalizacionNegocio = negocio == null ? null : negocio.CGeolocalizacion
            }).ToListAsync();
    }
}
