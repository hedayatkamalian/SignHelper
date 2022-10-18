namespace SignHelperApp.DTO
{
    public class SignRequestDto
    {
        public long Id { get; set; }
        public string ConfirmCode { get; set; }
        public string Description { get; set; }
        public string ExpireIn { get; set; }
        public string Email { get; set; }
    }
}
