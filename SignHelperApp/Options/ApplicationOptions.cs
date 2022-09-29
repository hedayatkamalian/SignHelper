namespace SignHelperApp.Options
{
    public class ApplicationOptions
    {
        public string ApplicationBaseUrl { get; set; }
        public string ConfirmApiAddress { get; set; }
        public NotifyOptions NotifyOptions { get; set; }
        public string DefaultSignImageName { get; set; }
        public FolderOptions Folders { get; set; }
        public ConfirmCodeOptions ConfirmCodeOptions { get; set; }
        public PrefixOptions PrefixOptions { get; set; }
        public SingedDocumentEmailOptions SignedDocumentEmailOptions { get; set; }
    }
}
