using CreditFlow.Web.Core.Http;
using CreditFlow.Web.Features.Maintenance.Agency.Models;

namespace CreditFlow.Web.Features.Maintenance.Agency.Services;

public interface IAgenciaService
{
    Task<ApiResult<List<AgenciaDto>>> ObtenerTodasAsync();

    Task<(bool Exito, string? Mensaje)> CrearAsync(CrearAgenciaRequest request);

    Task<(bool Exito, string? Mensaje)> ActualizarAsync(int id, ActualizarAgenciaRequest request);

    Task<(bool Exito, string? Mensaje)> EliminarAsync(int id);
}
