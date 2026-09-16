using CreditFlow.Web.Core.Validation;
using CreditFlow.Web.Features.Maintenance.Role.Models;

namespace CreditFlow.Web.Features.Maintenance.Role.Validation;

public static class RoleValidator
{
    public static UiValidationResult Validate(RoleDto role) =>
        string.IsNullOrWhiteSpace(role.Nombre)
            ? UiValidationResult.Failure("El nombre del rol es obligatorio.")
            : UiValidationResult.Success();
}
