using SignHelperApp.Entities;

namespace SignHelperApp.Commands.Templates
{
    public class TemplateAddCommand
    {
        public TemplateAddCommand(string name, IList<SignPoint> signPoints, string? imageName, int width, int height)
        {
            Name = name;
            ImageName = imageName;
            SignPoints = signPoints;
            Width = width;
            Height = height;
        }

        public string Name { get; private set; }
        public string? ImageName { get; private set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public IList<SignPoint> SignPoints { get; set; }


    }
}
