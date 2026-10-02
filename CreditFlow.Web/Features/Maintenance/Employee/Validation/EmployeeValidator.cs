using CreditFlow.Web.Core.Utils.Format;
using CreditFlow.Web.Core.Validation;
using CreditFlow.Web.Features.Maintenance.Employee.Models;
using CreditFlow.Web.Shared.Identity.Validation;
using Microsoft.AspNetCore.Components.Forms;

namespace CreditFlow.Web.Features.Maintenance.Employee.Validation;

/// <summary>Errores por campo para la validación en vivo del formulario de empleado.</summary>
public sealed record EmployeeFormErrors(
    string? Documento,
    string? Nombres,
    string? PrimerApellido,
    string? Sexo,
    string? Agencia,
    string? Correo,
    string? Password,
    string? Rol)
{
    public static readonly EmployeeFormErrors Ninguno = new(null, null, null, null, null, null, null, null);

    public bool HayErrores =>
        Documento is not null || Nombres is not null || PrimerApellido is not null || Sexo is not null ||
        Agencia is not null || Correo is not null || Password is not null || Rol is not null;
}

public static class EmployeeValidator
{
    public const long MaxPhotoSizeBytes = 5 * 1024 * 1024;

    private static readonly string[] AllowedPhotoTypes = ["image/jpeg", "image/png", "image/webp"];

    public static EmployeeFormErrors Validate(EmpleadoFormulario employee, bool isNew)
    {
        string? documento = null;
        if (isNew)
        {
            if (string.IsNullOrWhiteSpace(employee.Documento))
                documento = "El documento es obligatorio.";
            else if (DocumentValidator.Validate(employee.Documento, DocumentType.Dui) is { IsValid: false } dui)
                documento = dui.Message;
        }

        string? correo = null;
        if (string.IsNullOrWhiteSpace(employee.Correo))
            correo = "El correo es obligatorio.";
        else if (!EmailValidator.IsValid(employee.Correo))
            correo = "El correo no es válido.";

        return new EmployeeFormErrors(
            documento,
            string.IsNullOrWhiteSpace(employee.Nombres) ? "Los nombres son obligatorios." : null,
            string.IsNullOrWhiteSpace(employee.PrimerApellido) ? "El primer apellido es obligatorio." : null,
            employee.Sexo <= 0 ? "Selecciona el sexo." : null,
            employee.CodAgencia <= 0 ? "Selecciona una agencia." : null,
            correo,
            isNew && string.IsNullOrWhiteSpace(employee.Password) ? "La contraseña es obligatoria." : null,
            employee.IdRol <= 0 ? "Selecciona un rol." : null);
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
