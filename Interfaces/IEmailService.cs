namespace Pronia.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string receiver, string body, string subject, bool isHtml = true);
    }
}
