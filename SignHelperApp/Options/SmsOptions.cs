namespace SignHelperApp.Options
{
    public class SmsOptions
    {
        public string Url { get; set; }
        public string CallerId { get; set; }
        public string Token { get; set; }
        public IList<SmsTemplate> Templates { get; set; }
    }
}
