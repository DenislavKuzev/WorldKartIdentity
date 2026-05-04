using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WorldKartIdentity.Database;
using WorldKartIdentity.ViewModel;

namespace WorldKartIdentity.Controllers
{
    [Authorize]
    public class BlogController : Controller
    {

        private readonly ApplicationDbContext db;
        private readonly UserManager<User> _userManager;



#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        public BlogController(ApplicationDbContext context, UserManager<User> userManager)
        {
            db = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Blogs(int hasToInclude = 0)// adding this parameter so you can view the blog from notifications with type new like 
        {
            var viewModel = new List<BlogViewModel>();

            var blogs = await db.Blogs.Take(100).Include(b => b.Author).OrderByDescending(b=> b.PublishedDate).ToListAsync();
            
            HashSet<int> likeIds = new HashSet<int>();

            
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                likeIds = (await db.BlogLikes
                    .Where(bl => bl.UserId == user.Id)
                    .Select(bl => bl.BlogId)
                    .ToListAsync())
                    .ToHashSet();
            }

            foreach (var b in blogs)
            {

                BlogViewModel bvm = new BlogViewModel(b);

                bvm.LikedByCurrentUser = likeIds.Contains(b.Id);
                viewModel.Add(bvm);
            }

            if (hasToInclude != 0)
            {
                var blogToFocus = await db.Blogs.FindAsync(hasToInclude);
                if (blogToFocus != null)
                {
                    var blogInView = viewModel.FirstOrDefault(b => b.Id == blogToFocus.Id);
                    if (blogInView != null)
                    {
                        blogInView.Focus = true;
                    }
                    else
                    {
                        var blogVM = new BlogViewModel(blogToFocus);
                        blogVM.Focus = true;
                        viewModel.Add(blogVM);
                    }
                }
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBlog(BlogViewModel blogVM)
        {
            var user = await _userManager.GetUserAsync(User);
            var blog = BlogViewModel.BlogVMToBlog(blogVM);

            blog.AuthorId = user.Id;
            await db.Blogs.AddAsync(blog);
            await db.SaveChangesAsync();

            return RedirectToAction("Blogs");

        }

        [HttpPost]
        public async Task<IActionResult> ToggleLike(int bid)
        {
            var userId = _userManager.GetUserId(User);
            string username = _userManager.GetUserName(User);

            var exists = await db.BlogLikes
                .AnyAsync(x => x.UserId == userId && x.BlogId == bid);
            var blog = await db.Blogs.FindAsync(bid);

            if (!exists)
            {
                db.BlogLikes.Add(new BlogLikes
                {
                    UserId = userId,
                    BlogId = bid
                });
                
                if (blog != null)
                {
                    blog.Likes += 1;
                }
                await db.SaveChangesAsync();

                string blogLink = $"<a class=\"text-decoration-none track-link\" href=\"{Url.Action("Blogs", "Blog", new { hasToInclude = blog.Id }, Request.Scheme)}\" >блог</a></li>";
                //await AddNotification(
                //    type: NotificationType.NewLike,
                //    message: $"{username} хареса вашия {blogLink}",
                //    targetUserId: blog.AuthorId
                //);
            }
            else
            {
                var userLike = await db.BlogLikes
                    .FirstOrDefaultAsync(x => x.UserId == userId && x.BlogId == bid);

                db.BlogLikes.Remove(userLike);
                if (blog != null && blog.Likes > 0)
                {
                    blog.Likes -= 1;
                }
                await db.SaveChangesAsync();
            }
                return Json(new { likes = blog.Likes });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            var blog = await db.Blogs.FindAsync(id);
            if (blog != null)
            {
                db.Blogs.Remove(blog);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Blogs", "Blog");
        }


        public async Task<int> AddNotification(NotificationType type, string message, string? targetUserId)
        {
            string title = "";
            if (type == NotificationType.NewTrack)
            {
                title = "Нова писта добавена";
            }
            else if (type == NotificationType.RequestApproved)
            {
                title = "Заявката ви за писта бе одобрена!";
            }
            else if (type == NotificationType.NewLike)
            {
                title = "Ново харесване на блог";
            }
            else if (type == NotificationType.NewComment)
            {
                title = "Нов коментар на блог";
            }

            var n = new Notification
            {
                Title = title,
                Message = message,
                Type = type,
                CreatedAt = DateTime.Now,
                UserId = targetUserId
            };
            await db.Notifications.AddAsync(n);
            await db.SaveChangesAsync();

            return n.Id;
        }

        
    }
}
