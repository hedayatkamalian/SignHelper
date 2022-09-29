namespace SignHelperApp.Options
{
    public class ApplicationErrors
    {
        public string NoRowAffected { get; set; }
        public string TemplateIdDoesNotExist { get; set; }
        public string DocumentIsSignedBefore { get; set; }
        public string SignRequestIsExpired { get; set; }
    }
}
