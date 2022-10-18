namespace SignHelperApp.Commands.SignRequests
{
    public class SignRequestAddCommand
    {
        public SignRequestAddCommand
            (string fileURL,
            string recipientEmail,
            string signerEmail,
            string signerPhoneNumber,
            long templateId,
            string? description)
        {
            FileURL = fileURL;
            RecipientEmail = recipientEmail;
            SingerEmail = signerEmail;
            SingerPhoneNumber = signerPhoneNumber;
            TemplateId = templateId;
            Description = description;
        }

        public string FileURL { get; private set; }
        public string RecipientEmail { get; set; }
        public string SingerEmail { get; private set; }
        public string SingerPhoneNumber { get; private set; }
        public long TemplateId { get; private set; }
        public string? Description { get; private set; }
    }
}
