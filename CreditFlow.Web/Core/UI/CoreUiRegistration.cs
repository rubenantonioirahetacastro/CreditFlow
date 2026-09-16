using CreditFlow.Web.Core.UI.Notifications;

namespace CreditFlow.Web.Core.UI;

public static class CoreUiRegistration
{
    public static IServiceCollection AddCoreUi(this IServiceCollection services)
    {
        services.AddScoped<ICdsNotificationService, CdsNotificationService>();
        return services;
    }
}
