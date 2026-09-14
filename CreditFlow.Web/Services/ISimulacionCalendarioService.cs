using CreditFlow.Web.Models;

namespace CreditFlow.Web.Services;

public interface ISimulacionCalendarioService
{
    Task<(bool Exito, string? Mensaje, SimularCalendarioResponse? Data)> SimularAsync(SimularCalendarioRequest request);
}
