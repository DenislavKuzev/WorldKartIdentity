using WorldKartIdentity.Database;

namespace WorldKartIdentity.ViewModel
{
    public class BlogViewModel
    {
        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public DateTime PublishDate { get; set; } = DateTime.Now;

        public int Likes { get; set; }

        public int Dislikes { get; set; }

        public string? PictureBase64 { get; set; }



        public BlogViewModel(BlogPost blog)
        {
            Title = blog.Title;
            Content = blog.Content;
            PublishDate = blog.PublishDate;
            Likes = blog.Likes;
            Dislikes = blog.Dislikes;
            PictureBase64 = blog.PictureBase64;
        }

        public static BlogPost BlogVMToTrack(BlogViewModel blogVM)
        {
            BlogPost blog = new BlogPost();
            blog.Title = blogVM.Title;
            blog.Content = blogVM.Content;
            blog.PublishDate = blogVM.PublishDate;
            blog.Likes = blogVM.Likes;
            blog.Dislikes = blogVM.Dislikes;
            blog.PictureBase64 = blogVM.PictureBase64;
            return blog;
        }
    }
}
