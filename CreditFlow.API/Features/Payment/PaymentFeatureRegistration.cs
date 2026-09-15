using CreditFlow.API.Features.Payment.Mobile.RegisterPayment;

namespace CreditFlow.API.Features.Payment;

public static class PaymentFeatureRegistration
{
    public static IServiceCollection AddPaymentFeature(this IServiceCollection services)
    {
        services.AddScoped<IRegisterPaymentHandler, RegisterPaymentHandler>();
        return services;
    }
}
