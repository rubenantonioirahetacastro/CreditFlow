using CreditFlow.Web.Features.Simulator.Services;

namespace CreditFlow.Web.Features.Simulator;

public static class SimulatorFeatureRegistration
{
    public static IServiceCollection AddSimulatorFeature(this IServiceCollection services)
    {
        services.AddScoped<ISimulacionCalendarioService, SimulacionCalendarioApiService>();
        return services;
    }
}
