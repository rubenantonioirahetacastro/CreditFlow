using CreditFlow.Web.Features.Simulator.Models;

namespace CreditFlow.Web.Features.Simulator.Services;

public interface ISimulacionCalendarioService
{
    Task<(bool Exito, string? Mensaje, SimularCalendarioResponse? Data)> SimularAsync(SimularCalendarioRequest request);
}
