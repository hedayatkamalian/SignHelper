namespace SignHelperApp.Entities
{
    public class SignRequest
    {
        public long Id { get; set; }
        public string FileName { get; set; }
        public long TemplateId { get; set; }
        public Template Template { get; set; }
        public string Email { get; set; }
        public string ConfirmCode { get; set; }
        public DateTimeOffset ExpireIn { get; set; }
        public bool Done { get; set; }
        public string? Description { get; set; }
    }
}
