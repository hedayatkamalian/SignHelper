namespace SignHelperApp.Commands.SignRequests
{
    public class SignRequestAddCommand
    {
        public SignRequestAddCommand(string fileURL, string email, long templateId, string? description)
        {
            FileURL = fileURL;
            Email = email;
            TemplateId = templateId;
            Description = description;
        }

        public string FileURL { get; private set; }
        public string Email { get; private set; }
        public long TemplateId { get; private set; }
        public string? Description { get; private set; }
    }
}
