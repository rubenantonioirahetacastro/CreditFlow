using CreditFlow.API.Core.Errors;

namespace CreditFlow.API.Features.Employee.Shared.Errors;

public static class EmployeeErrors
{
    public static readonly ErrorDefinition CredentialsRequired = new(
        "employee_credentials_required",
        "El documento y la contraseña son obligatorios.");

    public static readonly ErrorDefinition NotFound = new(
        "employee_not_found",
        "El empleado no existe.");

    public static ErrorDefinition UserAlreadyExists(string document) => new(
        "employee_user_already_exists",
        $"Ya existe un usuario con el documento '{document}'.");

    public static ErrorDefinition RoleNotFound(int roleId) => new(
        "employee_role_not_found",
        $"El rol con IdRol {roleId} no existe.");

    public static ErrorDefinition InvalidPhotoType(string extension, IEnumerable<string> allowedExtensions) => new(
        "employee_invalid_photo_type",
        $"Tipo de archivo no permitido: {extension}. Formatos aceptados: {string.Join(", ", allowedExtensions)}.");

    public static ErrorDefinition PhotoTooLarge(long maximumMegabytes) => new(
        "employee_photo_too_large",
        $"La foto excede el tamaño máximo permitido ({maximumMegabytes}MB).");
}
