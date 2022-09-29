namespace SignHelperApp.Types
{
    public class Email
    {
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public IList<string>? AttachmentsUrl { get; set; } = new List<string>();
        public string HeaderPictureUrl { get; set; }
        public IList<string>? To { get; set; } = new List<string>();
        public IList<string>? Cc { get; set; } = new List<string>();
        public IList<string>? Bcc { get; set; } = new List<string>();

    }
}
