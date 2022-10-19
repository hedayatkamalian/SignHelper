using System.ComponentModel.DataAnnotations;

namespace SignHelper.Requests.SignRequests
{
    public class SignRequestAddRequest
    {
        [Required]
        public string FileURL { get; set; }

        [Required]
        [EmailAddress]
        public string RecipientEmail { get; set; }

        [Required]
        [EmailAddress]
        public string SingerEmail { get; set; }

        [Required]
        public string SignerPhoneNumber { get; set; }

        [Required]
        public long TemplateId { get; set; }
        public string? Description { get; set; }
    }
}
