using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WorldKartIdentity.Database;
using WorldKartIdentity.ViewModel;

namespace WorldKartIdentity.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<User> userManager;
        public AdminController(ApplicationDbContext context, UserManager<User> userManager)
        {
            db = context;
            this.userManager = userManager;
        }

        public IActionResult Admin()
        {
            ViewBag.KartingTracks = 0;
            ViewBag.Trajectories = 0;
            ViewBag.Users = 0;
            return View();
        }

        public async Task<IActionResult> Users()
        {
            var users = userManager.Users.ToList();
            var model = new List<UserViewModel>();
            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                model.Add(new UserViewModel
                {
                     Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Country = user.Country,
                    Bio = user.Bio,
                    RoleInKarting = user.RoleInKarting,
                    PhoneNumber = user.PhoneNumber,
                    Picture = user.Picture,
                    IsAdmin = roles.Contains("Admin")
                });
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> MakeAdmin(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
            await db.SaveChangesAsync();
            return RedirectToAction("Users", "Admin");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                await userManager.DeleteAsync(user);
            }
            await db.SaveChangesAsync();
            return RedirectToAction("Users", "Admin");
        }

        [HttpGet]
        public IActionResult TrackRequests()
        {
            var requests = db.TrackRequests.ToList();

            var model = requests.Select(r => new TrackRequestViewModel
            {
                Id = r.Id, Name = r.Name, Country = r.Country, LocationUrl = r.LocationUrl
            }).ToList();
            return View(model);
        }

        [HttpGet]
        public IActionResult Tracks()
        {
            var tracks = db.Tracks.ToList();
            var model = tracks.Select(t => new TrackViewModel
            {
                Id = t.Id,
                Name = t.Name,
                PictureBase64 = t.Picture,
                Location = t.Location,
                Email = t.Email,
                TelNumber = t.TelNumber,
                Worktime = t.Worktime
            }).ToList();

            return View(model);
        }

    }
}
