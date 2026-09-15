using CreditFlow.API.Features.Credit.Mobile.CreateCredit;
using CreditFlow.API.Features.Credit.Shared.Calendar;
using CreditFlow.API.Features.Credit.Shared.CreditLine;
using CreditFlow.API.Features.Credit.Web.ManageCreditLines;
using CreditFlow.API.Features.Credit.Web.UpdateCreditDecision;

namespace CreditFlow.API.Features.Credit;

public static class CreditFeatureRegistration
{
    public static IServiceCollection AddCreditFeature(this IServiceCollection services)
    {
        services.AddScoped<ICrearSolicitudCreditoHandler, CrearSolicitudCreditoHandler>();
        services.AddScoped<ICalendarioService, CalendarioService>();
        services.AddScoped<IHolidayService, HolidayService>();
        services.AddScoped<IExpenseService, ExpenseService>();
        services.AddScoped<IPrimaryCreditLineService, PrimaryCreditLineService>();
        services.AddScoped<ILineaCreditoService, LineaCreditoService>();
        services.AddScoped<ICreditLineManagementService, CreditLineManagementService>();
        services.AddScoped<IUpdateCreditDecisionHandler, UpdateCreditDecisionHandler>();
        return services;
    }
}
