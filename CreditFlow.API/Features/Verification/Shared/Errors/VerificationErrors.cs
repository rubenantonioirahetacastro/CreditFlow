using CreditFlow.API.Core.Errors;

namespace CreditFlow.API.Features.Verification.Shared.Errors;

public static class VerificationErrors
{
    public static readonly ErrorDefinition CreditNotFound = new(
        "verification_credit_not_found",
        "No se encontró la solicitud indicada.");

    public static readonly ErrorDefinition CreditAlreadyProcessed = new(
        "verification_credit_already_processed",
        "La solicitud ya fue procesada o no se encuentra en análisis.");

    public static readonly ErrorDefinition AlreadyRegistered = new(
        "verification_already_registered",
        "El crédito ya tiene una verificación registrada.");

    public static readonly ErrorDefinition PersonNotAssigned = new(
        "verification_person_not_assigned",
        "La solicitud no tiene una persona asociada.");

    public static readonly ErrorDefinition PersonNotFound = new(
        "verification_person_not_found",
        "No se encontró la persona asociada a la solicitud.");

    public static readonly ErrorDefinition EmployeeNotFound = new(
        "verification_employee_not_found",
        "No se encontró el empleado autenticado.");

    public static readonly ErrorDefinition BusinessRequired = new(
        "verification_business_required",
        "La verificación requiere los datos del negocio.");
}
