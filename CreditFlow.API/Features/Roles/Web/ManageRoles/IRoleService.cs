namespace CreditFlow.API.Features.Roles.Web.ManageRoles
{
    public interface IRoleService
    {
        Task<List<RoleDto>> GetActiveAsync();

        Task<List<RoleDto>> GetAllAsync();

        Task<RoleDto> CreateAsync(CreateRoleRequest request);

        Task<RoleDto?> UpdateAsync(int roleId, UpdateRoleRequest request);
    }
}
