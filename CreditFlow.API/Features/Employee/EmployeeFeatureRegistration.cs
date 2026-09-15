using CreditFlow.API.Features.Employee.Mobile.GetEmployeeHome;
using CreditFlow.API.Features.Employee.Web.ManageEmployees;

namespace CreditFlow.API.Features.Employee;

public static class EmployeeFeatureRegistration
{
    public static IServiceCollection AddEmployeeFeature(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IGetEmployeeHomeHandler, GetEmployeeHomeHandler>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.Configure<EmployeeHomeOptions>(
            configuration.GetSection(EmployeeHomeOptions.SectionName));

        return services;
    }
}
