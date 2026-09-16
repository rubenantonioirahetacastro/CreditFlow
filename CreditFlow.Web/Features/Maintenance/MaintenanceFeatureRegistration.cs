using CreditFlow.Web.Features.Maintenance.Agency.Services;
using CreditFlow.Web.Features.Maintenance.Catalog.Services;
using CreditFlow.Web.Features.Maintenance.CreditLine.Services;
using CreditFlow.Web.Features.Maintenance.Employee.Services;
using CreditFlow.Web.Features.Maintenance.Role.Services;

namespace CreditFlow.Web.Features.Maintenance;

public static class MaintenanceFeatureRegistration
{
    public static IServiceCollection AddMaintenanceFeature(this IServiceCollection services)
    {
        services.AddScoped<IAgenciaService, AgenciaApiService>();
        services.AddScoped<ICatalogoCodigoService, CatalogoCodigoApiService>();
        services.AddScoped<ILineaCreditoService, LineaCreditoApiService>();
        services.AddScoped<IEmpleadoService, EmpleadoApiService>();
        services.AddScoped<IRoleService, RoleApiService>();
        return services;
    }
}
