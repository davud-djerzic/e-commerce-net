using SendGrid.Helpers.Mail;
using SendGrid;
using Ecommerce.Services.ServiceInterfaces;

namespace Ecommerce.Services
{
    public class SendGridService : ISendGridService
    {
        private readonly string sendGridApiKey;

        public SendGridService(IConfiguration configuration)
        {
            sendGridApiKey = configuration["SendGrid:ApiKey"];
        }

        public async Task SendEmailAsync(string senderName, string receiverName, string subjectText, string receiverEmail, string text, string path)
        {
            //string apiKey = Environment.GetEnvironmentVariable("SendGrid_ApiKey");
            if (string.IsNullOrEmpty(sendGridApiKey))
            {
                throw new Exception("SendGrid API key is not configured.");
            }
            var client = new SendGridClient(sendGridApiKey);
            var from = new EmailAddress("djerzicd831@gmail.com", senderName);
            var subject = subjectText;
            var to = new EmailAddress(receiverEmail, receiverEmail);
            var plainTextContent = text;
            var htmlContent = text;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            if (File.Exists(path))
            {
                byte[] fileBytes = File.ReadAllBytes(path);
                string fileBase64 = Convert.ToBase64String(fileBytes);

                msg.AddAttachment(Path.GetFileName(path), fileBase64);
            }
            else
            {
                throw new Exception($"Failed to send email: {msg}");
            }

            msg.MailSettings = new MailSettings
            {
                SandboxMode = new SandboxMode { Enable = false }
            };
            var response = await client.SendEmailAsync(msg);
            if (response.StatusCode != System.Net.HttpStatusCode.OK &&
               response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                var responseBody = await response.Body.ReadAsStringAsync();
                throw new Exception($"Failed to send email: {response.StatusCode} - {responseBody}");
            }
        }
    }
}
