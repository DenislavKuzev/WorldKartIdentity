using WorldKartIdentity.ViewModel;

namespace WorldKartIdentity.Database
{
    public class BlogPost
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public string AuthorId { get; set; } = null!;

        public DateTime PublishedDate { get; set; } = DateTime.Now;

        public int Likes { get; set; }

        public int Dislikes { get; set; } 

        public string? PictureBase64 { get; set; }

        public User Author { get; set; } = null!;

        public List<BlogLikes> BlogLikes { get; set; } = new List<BlogLikes>();


    }
}
