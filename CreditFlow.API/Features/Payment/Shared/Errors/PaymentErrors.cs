using CreditFlow.API.Core.Errors;

namespace CreditFlow.API.Features.Payment.Shared.Errors;

public static class PaymentErrors
{
    public static readonly ErrorDefinition InvalidAmount = new(
        "payment_invalid_amount",
        "El monto a abonar debe ser mayor que cero.");

    public static readonly ErrorDefinition CreditNotFound = new(
        "payment_credit_not_found",
        "No se encontró el crédito indicado.");

    public static readonly ErrorDefinition CalendarNotAssigned = new(
        "payment_calendar_not_assigned",
        "El crédito no tiene una condición de calendario asociada.");

    public static readonly ErrorDefinition CalendarNotFound = new(
        "payment_calendar_not_found",
        "No se encontró la condición de calendario del crédito.");

    public static readonly ErrorDefinition NoPendingInstallments = new(
        "payment_no_pending_installments",
        "El crédito no tiene cuotas pendientes en el calendario actual.");
}
