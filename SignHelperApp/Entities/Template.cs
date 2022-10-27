namespace SignHelperApp.Entities
{
    public class Template
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? StampName { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public IList<SignPoint> SignPoints { get; set; }
        public IList<SignRequest> SignRequests { get; set; }
        public bool Deleted { get; set; }
    }
}
