namespace CreditFlow.API.Core.Email
{
    public interface IEmailService
    {
        Task SendAsync(string toEmail, string subject, string body);
    }
}
