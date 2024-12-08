namespace Ecommerce.Services.ServiceInterfaces
{
    public interface ISendGridService
    {
        Task SendEmailAsync(string senderName, string receiverName, string subjectText, string receiverMail, string text, string path);
    }
}
