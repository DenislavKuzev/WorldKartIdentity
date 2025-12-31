namespace WorldKartIdentity.Database
{
    public class BlogLikes
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public User User { get; set; }

        public int BlogId { get; set; }

        public BlogPost Blog { get; set; }
    }
}
