using CreditFlow.API.Features.Roles.Shared.Permissions;
using CreditFlow.API.Features.Roles.Web.ManageRoles;

namespace CreditFlow.API.Features.Roles;

public static class RoleFeatureRegistration
{
    public static IServiceCollection AddRoleFeature(this IServiceCollection services)
    {
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IRolePermissionService, RolePermissionService>();
        return services;
    }
}
