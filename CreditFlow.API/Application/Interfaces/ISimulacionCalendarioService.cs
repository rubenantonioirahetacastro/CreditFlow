using CreditFlow.API.Application.Requests;
using CreditFlow.API.Application.DTOs;

namespace CreditFlow.API.Application.Interfaces
{
    public interface ISimulacionCalendarioService
    {
        Task<SimularCalendarioResponse> SimularAsync(SimularCalendarioRequest request);
    }
}
