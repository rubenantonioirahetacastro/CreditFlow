using CreditFlow.Web.Core.Utils.Format;
using CreditFlow.Web.Core.Validation;
using CreditFlow.Web.Features.Maintenance.Employee.Models;
using CreditFlow.Web.Shared.Identity.Validation;
using Microsoft.AspNetCore.Components.Forms;

namespace CreditFlow.Web.Features.Maintenance.Employee.Validation;

/// <summary>Errores por campo para la validación en vivo del panel de empleados.</summary>
public sealed record EmployeeFormErrors(
    string? Documento,
    string? Nombres,
    string? PrimerApellido,
    string? Sexo,
    string? Correo,
    string? Telefono,
    string? Agencia,
    string? Rol,
    string? Password)
{
    public static readonly EmployeeFormErrors Ninguno = new(null, null, null, null, null, null, null, null, null);

    public bool HayErrores =>
        Documento is not null || Nombres is not null || PrimerApellido is not null || Sexo is not null ||
        Correo is not null || Telefono is not null || Agencia is not null || Rol is not null || Password is not null;
}

public static class EmployeeValidator
{
    public const long MaxPhotoSizeBytes = 5 * 1024 * 1024;

    private static readonly string[] AllowedPhotoTypes = ["image/jpeg", "image/png", "image/webp"];

    public static EmployeeFormErrors Validate(EmpleadoFormModel employee)
    {
        string? documento = null;
        if (employee.EsNuevo)
        {
            documento = string.IsNullOrWhiteSpace(employee.Documento)
                ? "El documento es obligatorio."
                : DocumentValidator.Validate(employee.Documento, DocumentType.Dui) is { IsValid: false } resultado
                    ? resultado.Message
                    : null;
        }

        var nombres = string.IsNullOrWhiteSpace(employee.Nombres) ? "Los nombres son obligatorios." : null;
        var primerApellido = string.IsNullOrWhiteSpace(employee.PrimerApellido) ? "El primer apellido es obligatorio." : null;
        var sexo = employee.Sexo <= 0 ? "Selecciona el sexo." : null;

        var correo = string.IsNullOrWhiteSpace(employee.Correo)
            ? "El correo es obligatorio."
            : !EmailValidator.IsValid(employee.Correo.Trim()) ? "El correo no es válido." : null;

        var telefonoDigitos = PhoneFormatter.Normalize(employee.Telefono);
        var telefono = telefonoDigitos.Length is > 0 and < 8 ? "El teléfono debe tener 8 dígitos." : null;

        var agencia = employee.CodAgencia <= 0 ? "Selecciona una agencia." : null;
        var rol = employee.IdRol <= 0 ? "Selecciona un rol." : null;
        var password = employee.EsNuevo && string.IsNullOrWhiteSpace(employee.Password)
            ? "La contraseña es obligatoria."
            : null;

        return new EmployeeFormErrors(documento, nombres, primerApellido, sexo, correo, telefono, agencia, rol, password);
    }

    public static UiValidationResult ValidatePhoto(IBrowserFile photo)
    {
        if (!AllowedPhotoTypes.Contains(photo.ContentType, StringComparer.OrdinalIgnoreCase))
            return UiValidationResult.Failure("La foto debe ser JPG, PNG o WEBP.");

        return photo.Size > MaxPhotoSizeBytes
            ? UiValidationResult.Failure("La foto no puede superar los 5 MB.")
            : UiValidationResult.Success();
    }
}
