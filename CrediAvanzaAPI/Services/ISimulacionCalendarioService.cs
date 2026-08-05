using CrediAvanzaAPI.Application.Requests;
using CrediAvanzaAPI.Application.DTOs;

namespace CrediAvanzaAPI.Services
{
    public interface ISimulacionCalendarioService
    {
        Task<SimularCalendarioResponse> SimularAsync(SimularCalendarioRequest request);
    }
}
