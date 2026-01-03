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



#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
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
            HashSet<int> likeIds = new HashSet<int>();

            if (User.Identity.IsAuthenticated)
            {
 
                string userId = _userManager.GetUserId(User);
                likeIds = (await db.BlogLikes
                    .Where(bl => bl.UserId == userId)
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
