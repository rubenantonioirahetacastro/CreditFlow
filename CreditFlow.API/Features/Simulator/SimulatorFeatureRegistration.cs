using CreditFlow.API.Features.Simulator.Shared.SimulateCalendar;

namespace CreditFlow.API.Features.Simulator;

public static class SimulatorFeatureRegistration
{
    public static IServiceCollection AddSimulatorFeature(this IServiceCollection services)
    {
        services.AddScoped<ISimulacionCalendarioService, SimulacionCalendarioService>();
        services.AddScoped<IBusinessVariableService, BusinessVariableService>();
        return services;
    }
}
