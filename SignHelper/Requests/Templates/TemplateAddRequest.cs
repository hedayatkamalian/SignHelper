using System.ComponentModel.DataAnnotations;

namespace SignHelper.Requests.Templates
{
    public class TemplateAddRequest
    {
        [Required]
        public string Name { get; set; }

        public string? ImageName { get; set; }

        [Required]
        public int Width { get; set; }

        [Required]
        public int Height { get; set; }

        [Required]
        public IList<SingPointAddRequest> SingPoints { get; set; }
    }
}
