namespace SignHelperApp.Commands.SignRequests
{
    public class SignRequestAddCommand
    {
        public SignRequestAddCommand
            (string fileURL,
            string recipientEmail,
            string signerEmail,
            string signerPhoneNumber,
            long singTemplateId,
            long? stampTemplateId,
            string? description)
        {
            FileURL = fileURL;
            RecipientEmail = recipientEmail;
            SingerEmail = signerEmail;
            SingerPhoneNumber = signerPhoneNumber;
            SignTemplateId = singTemplateId;
            StampTemplateId = singTemplateId;
            Description = description;
        }

        public string FileURL { get; private set; }
        public string RecipientEmail { get; set; }
        public string SingerEmail { get; private set; }
        public string SingerPhoneNumber { get; private set; }
        public long SignTemplateId { get; private set; }
        public long? StampTemplateId { get; private set; }
        public string? Description { get; private set; }
    }
}
