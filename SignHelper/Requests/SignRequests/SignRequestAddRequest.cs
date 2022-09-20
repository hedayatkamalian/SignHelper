using System.ComponentModel.DataAnnotations;

namespace SignHelper.Requests.SignRequests
{
    public class SignRequestAddRequest
    {
        [Required]
        public string FileURL { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public long TemplateId { get; set; }
        public string? Description { get; set; }
    }
}
