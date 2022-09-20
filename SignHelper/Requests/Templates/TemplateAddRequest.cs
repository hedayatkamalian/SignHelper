using System.ComponentModel.DataAnnotations;

namespace SignHelper.Requests.Templates
{
    public class TemplateAddRequest
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public int XPosition { get; set; }

        [Required]
        public int YPosition { get; set; }
    }
}
