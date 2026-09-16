using Radzen;

namespace CreditFlow.Web.Core.UI.Notifications;

public sealed class CdsNotificationService(NotificationService notificationService) : ICdsNotificationService
{
    public void Success(string title, string? message = null) =>
        Notify(NotificationSeverity.Success, title, message);

    public void Error(string title, string? message = null) =>
        Notify(NotificationSeverity.Error, title, message, 7000);

    public void Warning(string title, string? message = null) =>
        Notify(NotificationSeverity.Warning, title, message, 6000);

    public void Info(string title, string? message = null) =>
        Notify(NotificationSeverity.Info, title, message);

    private void Notify(NotificationSeverity severity, string title, string? message, double duration = 4500)
    {
        notificationService.Notify(new NotificationMessage
        {
            Severity = severity,
            Summary = title,
            Detail = message ?? string.Empty,
            Duration = duration,
            ShowProgress = true,
            CloseOnClick = false
        });
    }
}
