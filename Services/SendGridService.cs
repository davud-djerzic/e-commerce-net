using SendGrid.Helpers.Mail;
using SendGrid;
using Ecommerce.Services.ServiceInterfaces;
using Ecommerce.Models;
using Ecommerce.Configs;
using Microsoft.Extensions.Options;

namespace Ecommerce.Services
{
    public class SendGridService : ISendGridService
    {
        private readonly SendGridConfig _config;

        public SendGridService(IOptions<SendGridConfig> config)
        {
            _config = config.Value;
        }

        public async Task SendEmailAsync(SendGridModel sendGrid)
        {
            //string apiKey = Environment.GetEnvironmentVariable("SendGrid_ApiKey");
            if (string.IsNullOrEmpty(_config.ApiKey))
            {
                throw new Exception("SendGrid API key is not configured.");
            }
            var client = new SendGridClient(_config.ApiKey);
            var from = new EmailAddress(_config.SenderEmail, sendGrid.senderName);
            var subject = sendGrid.subjectText;
            var to = new EmailAddress(sendGrid.receiverEmail, sendGrid.receiverName);
            var plainTextContent = sendGrid.text;
            var htmlContent = sendGrid.text;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            if (File.Exists(sendGrid.path))
            {
                byte[] fileBytes = File.ReadAllBytes(sendGrid.path);
                string fileBase64 = Convert.ToBase64String(fileBytes);

                msg.AddAttachment(Path.GetFileName(sendGrid.path), fileBase64);
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
