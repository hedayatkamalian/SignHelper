namespace SignHelperApp.Command.Template
{
    public class TemplateAddCommand
    {
        public TemplateAddCommand(string name, int xPosition, int yPosition)
        {
            Name = name;
            XPosition = xPosition;
            YPosition = yPosition;
        }

        public string Name { get; private set; }
        public int XPosition { get; private set; }
        public int YPosition { get; private set; }

    }
}
