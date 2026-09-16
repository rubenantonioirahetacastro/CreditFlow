namespace CreditFlow.Web.Core.UI.Notifications;

public interface ICdsNotificationService
{
    void Success(string title, string? message = null);
    void Error(string title, string? message = null);
    void Warning(string title, string? message = null);
    void Info(string title, string? message = null);
}
