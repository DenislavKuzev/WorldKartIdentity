using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WorldKartIdentity.Database;
using WorldKartIdentity.ViewModel;

namespace WorldKartIdentity.Controllers
{
    public class BlogController : Controller
    {

        private readonly ApplicationDbContext db;
        private readonly UserManager<User> _userManager;

        public BlogController(ApplicationDbContext context, UserManager<User> userManager)
        {
            db = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Blogs()
        {
            var viewModel = new List<BlogViewModel>();
            var blogs = await db.Blogs.Take(100).Include(b => b.Author).ToListAsync();
            blogs.ForEach(async(b) =>
            {
                BlogViewModel bvm = new BlogViewModel(b);
                if(User.Identity.IsAuthenticated)
                {
                    var userId = _userManager.GetUserId(User);
                    var liked = await db.BlogLikes.AnyAsync(bl => bl.BlogId == b.Id && bl.UserId == userId);
                    bvm.LikedByCurrentUser = liked;
                    viewModel.Add(bvm);
                }
            });

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

    }
}
