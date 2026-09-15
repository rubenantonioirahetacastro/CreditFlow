using CreditFlow.API.Core.Errors;

namespace CreditFlow.API.Features.Simulator.Shared.Errors;

public static class SimulatorErrors
{
    public static readonly ErrorDefinition InvalidAmount = new(
        "simulator_invalid_amount",
        "El monto solicitado debe ser mayor que cero.");

    public static readonly ErrorDefinition InvalidTerm = new(
        "simulator_invalid_term",
        "El plazo debe ser mayor que cero.");

    public static readonly ErrorDefinition AgencyRequired = new(
        "simulator_agency_required",
        "La agencia es obligatoria para resolver la línea de crédito.");

    public static readonly ErrorDefinition AssignedCreditLineNotFound = new(
        "simulator_assigned_credit_line_not_found",
        "No se encontró la línea de crédito asignada.");

    public static readonly ErrorDefinition CreditLineNotConfigured = new(
        "simulator_credit_line_not_configured",
        "No existe una línea de crédito configurada para la agencia, producto, subproducto, plazo, monto, moneda y campaña indicados.");
}
