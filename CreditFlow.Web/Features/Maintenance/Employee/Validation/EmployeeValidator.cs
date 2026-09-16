using CreditFlow.Web.Core.Utils.Format;
using CreditFlow.Web.Core.Validation;
using CreditFlow.Web.Features.Maintenance.Employee.Models;
using CreditFlow.Web.Shared.Identity.Validation;
using Microsoft.AspNetCore.Components.Forms;

namespace CreditFlow.Web.Features.Maintenance.Employee.Validation;

public static class EmployeeValidator
{
    public const long MaxPhotoSizeBytes = 5 * 1024 * 1024;

    public static UiValidationResult Validate(EmpleadoDto employee, bool isNew, string? password)
    {
        if (isNew && string.IsNullOrWhiteSpace(employee.Documento))
            return UiValidationResult.Failure("El documento es obligatorio.");

        if (isNew)
        {
            var documentValidation = DocumentValidator.Validate(employee.Documento, DocumentType.Dui);
            if (!documentValidation.IsValid)
                return documentValidation;
        }

        if (string.IsNullOrWhiteSpace(employee.Nombres))
            return UiValidationResult.Failure("Los nombres son obligatorios.");

        if (string.IsNullOrWhiteSpace(employee.PrimerApellido))
            return UiValidationResult.Failure("El primer apellido es obligatorio.");

        if (employee.Sexo <= 0)
            return UiValidationResult.Failure("Debe seleccionar el sexo.");

        if (employee.CodAgencia <= 0)
            return UiValidationResult.Failure("Debe seleccionar una agencia.");

        if (string.IsNullOrWhiteSpace(employee.Correo))
            return UiValidationResult.Failure("El correo es obligatorio.");

        if (!EmailValidator.IsValid(employee.Correo))
            return UiValidationResult.Failure("El correo no es válido.");

        if (isNew && string.IsNullOrWhiteSpace(password))
            return UiValidationResult.Failure("La contraseña es obligatoria.");

        if (employee.IdRol <= 0)
            return UiValidationResult.Failure("Debe seleccionar un rol.");

        return UiValidationResult.Success();
    }

    public static UiValidationResult ValidatePhoto(IBrowserFile photo) =>
        photo.Size > MaxPhotoSizeBytes
            ? UiValidationResult.Failure("La foto no puede superar los 5MB.")
            : UiValidationResult.Success();
}
