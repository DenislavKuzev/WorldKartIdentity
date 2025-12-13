using Microsoft.AspNetCore.Mvc;

namespace WorldKartIdentity.Controllers
{
    public class BlogController : Controller
    {
        public IActionResult Blogs()
        {
            return View();
        }
    }
}
