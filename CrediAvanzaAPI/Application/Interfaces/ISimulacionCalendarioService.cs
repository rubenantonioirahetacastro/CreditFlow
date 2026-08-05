using CrediAvanzaAPI.Application.Requests;
using CrediAvanzaAPI.Application.DTOs;

namespace CrediAvanzaAPI.Application.Interfaces
{
    public interface ISimulacionCalendarioService
    {
        Task<SimularCalendarioResponse> SimularAsync(SimularCalendarioRequest request);
    }
}
