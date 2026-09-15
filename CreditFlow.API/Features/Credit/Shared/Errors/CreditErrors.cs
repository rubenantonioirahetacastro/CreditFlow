using CreditFlow.API.Core.Errors;

namespace CreditFlow.API.Features.Credit.Shared.Errors;

public static class CreditErrors
{
    public static readonly ErrorDefinition GuaranteeRequired = new(
        "credit_guarantee_required",
        "La garantía es obligatoria.");

    public static readonly ErrorDefinition GuaranteeTypeRequired = new(
        "credit_guarantee_type_required",
        "Debe seleccionar el tipo de garantía.");

    public static readonly ErrorDefinition GuaranteeBrandRequired = new(
        "credit_guarantee_brand_required",
        "Debe seleccionar la marca de la garantía.");

    public static readonly ErrorDefinition GuaranteeYearRequired = new(
        "credit_guarantee_year_required",
        "Debe seleccionar el año de la garantía.");

    public static readonly ErrorDefinition InvalidDecisionStatus = new(
        "credit_invalid_decision_status",
        "El estado indicado no está permitido como decisión de evaluación.");

    public static readonly ErrorDefinition NotFound = new(
        "credit_not_found",
        "No se encontró el crédito indicado.");

    public static readonly ErrorDefinition VerificationRequired = new(
        "credit_verification_required",
        "La solicitud debe estar verificada antes de poder aprobarse.");

    public static readonly ErrorDefinition CreditLineNotFound = new(
        "credit_line_not_found",
        "Línea de crédito no encontrada.");

    public static readonly ErrorDefinition CreditLineDescriptionRequired = new(
        "credit_line_description_required",
        "La descripción de la línea es requerida.");

    public static readonly ErrorDefinition CreditLineDescriptionTooLong = new(
        "credit_line_description_too_long",
        "La descripción no puede superar los 150 caracteres.");

    public static readonly ErrorDefinition InvalidCreditLineTermRange = new(
        "credit_line_invalid_term_range",
        "El plazo mínimo no puede ser mayor al plazo máximo.");

    public static readonly ErrorDefinition InvalidCreditLineAmountRange = new(
        "credit_line_invalid_amount_range",
        "El monto mínimo no puede ser mayor al monto máximo.");

    public static readonly ErrorDefinition CreditLineInUse = new(
        "credit_line_in_use",
        "No se puede eliminar la línea de crédito porque está en uso. Desactívela en su lugar.");

    public static ErrorDefinition RequestAlreadyExists(string document) => new(
        "credit_request_already_exists",
        $"Ya existe una solicitud registrada para el documento {document}.");

    public static ErrorDefinition TermOutsideCreditLine(int term, int minimum, int maximum) => new(
        "credit_term_outside_line",
        $"El plazo {term} está fuera del rango permitido ({minimum}-{maximum}) para este producto.");

    public static readonly ErrorDefinition ClientRoleUnavailable = new(
        "credit_client_role_unavailable",
        "El rol de cliente no existe o está inactivo.");

    public static readonly ErrorDefinition CalendarConditionNotFound = new(
        "credit_calendar_condition_not_found",
        "No se encontraron las condiciones de calendario del crédito.");

    public static readonly ErrorDefinition CalendarCreditLineNotFound = new(
        "credit_calendar_line_not_found",
        "No se encontró la línea de crédito para obtener la tasa.");

    public static readonly ErrorDefinition ProjectionCreditLineNotConfigured = new(
        "credit_projection_line_not_configured",
        "No existe una línea de crédito configurada para el subproducto, monto y plazo proporcionados.");

    public static ErrorDefinition InvalidSubProduct(int subProduct) => new(
        "credit_invalid_subproduct",
        $"El subproducto {subProduct} no está definido.");

    public static ErrorDefinition CreditLineNotConfigured(int subProduct, decimal amount) => new(
        "credit_line_not_configured",
        $"No existe una línea de crédito para el subproducto {subProduct} con monto ${amount}.");
}
