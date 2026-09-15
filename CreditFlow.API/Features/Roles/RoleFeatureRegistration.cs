using CreditFlow.API.Features.Roles.Web.ManageRoles;

namespace CreditFlow.API.Features.Roles;

public static class RoleFeatureRegistration
{
    public static IServiceCollection AddRoleFeature(this IServiceCollection services)
    {
        services.AddScoped<IRoleService, RoleService>();
        return services;
    }
}
