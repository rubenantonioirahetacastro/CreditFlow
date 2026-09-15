namespace CreditFlow.API.Features.Simulator.Shared.SimulateCalendar
{
    public interface ISimulacionCalendarioService
    {
        Task<SimularCalendarioResponse> SimularAsync(SimularCalendarioRequest request);
    }
}
