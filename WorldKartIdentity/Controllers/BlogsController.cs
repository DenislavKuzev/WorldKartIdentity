using Microsoft.AspNetCore.Mvc;

namespace WorldKartIdentity.Controllers
{
    public class BlogsController : Controller
    {
        public IActionResult Blogs()
        {
            return View();
        }
    }
}
