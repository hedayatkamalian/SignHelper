namespace SignHelperApp.Entities
{
    public class Template
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int XPosition { get; set; }
        public int YPosition { get; set; }
        public IList<SignRequest> SignRequests { get; set; }
        public bool Deleted { get; set; }
    }
}
