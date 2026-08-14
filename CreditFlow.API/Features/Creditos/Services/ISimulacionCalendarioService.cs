using CreditFlow.API.Features.Creditos.Requests;
using CreditFlow.API.Features.Creditos.DTOs;

namespace CreditFlow.API.Features.Creditos.Services
{
    public interface ISimulacionCalendarioService
    {
        Task<SimularCalendarioResponse> SimularAsync(SimularCalendarioRequest request);
    }
}
