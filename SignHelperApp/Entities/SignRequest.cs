namespace SignHelperApp.Entities
{
    public class SignRequest
    {
        public long Id { get; set; }
        public Template SignTemplate { get; set; }
        public long SignTemplateId { get; set; }
        public long? StampTemplateId { get; set; }

        public string RecipientEmail { get; set; }
        public string SignerEmail { get; set; }
        public string SignerPhoneNumber { get; set; }
        public string ConfirmCode { get; set; }
        public DateTimeOffset? ConfirmCodeExpireIn { get; set; }
        public DateTimeOffset? ExpireIn { get; set; }
        public bool Done { get; set; }
        public string? Description { get; set; }
    }
}
