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
            var viewModel = new List<BlogViewModel>();
            var blogs = await db.Blogs.Take(100).ToListAsync();
            blogs.ForEach(b => viewModel.Add(new BlogViewModel(b)));

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBlog(BlogViewModel blogVM)
        {
            var blog = BlogViewModel.BlogVMToBlog(blogVM);
            await db.Blogs.AddAsync(blog);
            await db.SaveChangesAsync();

            return RedirectToAction("Blogs");

        }

    }
}
