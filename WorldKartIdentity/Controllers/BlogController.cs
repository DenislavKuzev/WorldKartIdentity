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

        public BlogController(ApplicationDbContext context)
        {
            db = context;
        }

        [HttpGet]
        public async Task<IActionResult> Blogs()
        {
            var blogs = await db.Blogs.Take(100).ToListAsync();
            return View(blogs);
        }

        //[HttpPost]
        //public async Task<IActionResult> CreateBlog(BlogViewModel blogVM)
        //{
        //    var blog = BlogViewModel.BlogVMToTrack(blogVM);
            
        //}
    }
}
