namespace WorldKartIdentity.Database
{
    public class BlogReport
    {
        public int? Id { get; set; }
        public int BlogId { get; set; }
        public BlogPost Blog { get; set; }
        public string? ReporterId { get; set; }
        public User Reporter { get; set; }
        public DateTime ReportedOn { get; set; } = DateTime.Now;

    }
}
