namespace SignHelperApp.Options
{
    public class SingedDocumentEmailOptions
    {
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public string Subject { get; set; }
        public string HeaderPictureUrl { get; set; }
        public string Body { get; set; }
        public IList<string>? To { get; set; }
        public IList<string>? Cc { get; set; }
        public IList<string>? Bcc { get; set; }

    }
}
