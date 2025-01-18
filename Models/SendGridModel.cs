namespace Ecommerce.Models
{
    public class SendGridModel
    {
        public string senderName { get; set; } = string.Empty;
        public string receiverName { get; set; } = string.Empty;
        public string subjectText { get; set; } = string.Empty;
        public string receiverEmail { get; set; } = string.Empty;
        public string text { get; set; } = string.Empty;
        public string path { get; set; } = string.Empty;
        public SendGridModel(string senderName, string receiverName, string subjectText, string receiverEmail, string text, string path) 
        {
            this.senderName = senderName;
            this.receiverName = receiverName;
            this.subjectText = subjectText;
            this.receiverEmail = receiverEmail;
            this.text = text;
            this.path = path;
        }

    }
}
