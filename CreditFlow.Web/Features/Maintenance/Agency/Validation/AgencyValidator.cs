using CreditFlow.Web.Core.Validation;
using CreditFlow.Web.Features.Maintenance.Agency.Models;

namespace CreditFlow.Web.Features.Maintenance.Agency.Validation;

public static class AgencyValidator
{
    public static UiValidationResult Validate(AgenciaDto agency, bool isNew)
    {
        if (isNew && agency.NCodAge <= 0)
            return UiValidationResult.Failure("El código de agencia debe ser mayor a cero.");

        if (string.IsNullOrWhiteSpace(agency.Nombre))
            return UiValidationResult.Failure("El nombre de la agencia es obligatorio.");

        if (!EmailValidator.IsValid(agency.CorreoElectronico))
        {
            return UiValidationResult.Failure("El correo electrónico no es válido.");
        }

        return UiValidationResult.Success();
    }
}
