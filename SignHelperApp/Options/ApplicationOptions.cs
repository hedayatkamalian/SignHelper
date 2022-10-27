namespace SignHelperApp.Options
{
    public class ApplicationOptions
    {
        public string ApplicationBaseUrl { get; set; }
        public string ConfirmApiAddress { get; set; }
        public int ExpireInDays { get; set; }
        public string SignCallbackUrl { get; set; }
        public string DefaultStampImageName { get; set; }
        public string ConfirmCodeTemplateName { get; set; }
        public FolderOptions Folders { get; set; }
        public ConfirmCodeOptions ConfirmCodeOptions { get; set; }
        public PrefixOptions PrefixOptions { get; set; }
        public EmailPatternOption SendDraftDocument { get; set; }
        public EmailPatternOption SendSignedDocument { get; set; }
    }
}
