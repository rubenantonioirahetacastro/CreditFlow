using CreditFlow.Web.Core.Http;
using CreditFlow.Web.Models;

namespace CreditFlow.Web.Services;

public sealed class SimulacionCalendarioApiService(IApiClient apiClient)
    : ISimulacionCalendarioService
{
    public async Task<(bool Exito, string? Mensaje, SimularCalendarioResponse? Data)> SimularAsync(
        SimularCalendarioRequest request)
    {
        var result = await apiClient.PostAsync<SimularCalendarioRequest, SimularCalendarioResponse>(
            "api/Credito/simular-calendario",
            request,
            "No se pudo generar el cronograma.");

        return (result.IsSuccess, result.Message, result.Data);
    }
}
