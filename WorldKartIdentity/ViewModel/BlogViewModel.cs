using WorldKartIdentity.Database;

namespace WorldKartIdentity.ViewModel
{
    public class BlogViewModel
    {

        public int Id { get; set; }
        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public User Author { get; set; } = null!;

        public DateTime PublishedDate { get; set; } = DateTime.Now;

        public int Likes { get; set; }

        public int Dislikes { get; set; }

        public string? PictureBase64 { get; set; }

        public IFormFile? PictureFile { get; set; }

        public bool LikedByCurrentUser { get; set; }



        public BlogViewModel()
        {
        }

        public BlogViewModel(BlogPost blog)
        {
            Id = blog.Id;
            Title = blog.Title;
            Content = blog.Content;
            PublishedDate = blog.PublishedDate;
            Likes = blog.Likes;
            Dislikes = blog.Dislikes;
            PictureBase64 = blog.PictureBase64;
            Author = blog.Author;
        }

        public static BlogPost BlogVMToBlog(BlogViewModel blogVM)
        {
            BlogPost blog = new BlogPost();
            blog.Title = blogVM.Title;
            blog.Content = blogVM.Content;
            blog.PublishedDate = blogVM.PublishedDate;
            blog.Likes = blogVM.Likes;
            blog.Dislikes = blogVM.Dislikes;

            if (blogVM.PictureFile != null) 
            {
                blog.PictureBase64 = FileToBase64(blogVM.PictureFile);
            }
            else
            {
                blog.PictureBase64 = string.Empty;
            }
                
            return blog;
        }

        public static string FileToBase64(IFormFile file)
        {
            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                byte[] streamBytes = stream.ToArray();

                return Convert.ToBase64String(streamBytes);
            }
            
        }
    }
}
