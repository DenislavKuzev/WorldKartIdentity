using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.Xml;
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

        public async Task<IActionResult> Admin()
        {
            var usersCreatedTrajectoriesCount = await db.TrackTrajectories
                .Where(tt => tt.UserId != null)
                .GroupBy(tt => tt.UserId)
                .CountAsync();
            var vm = new AdminViewModel
            {
                TracksCount = await db.Tracks.CountAsync(),
                TrackTrajectoriesCount = await db.TrackTrajectories.CountAsync(),
                UsersCreatedTrajectoriesCount = usersCreatedTrajectoriesCount
            };
            return View(vm);
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
            var currentUserId = userManager.GetUserId(User);
            if (id == currentUserId)
            {
                return RedirectToAction("Users", "Admin");
            }

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
                Id = r.Id,
                Name = r.Name,
                Country = r.Country,
                LocationUrl = r.LocationUrl
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
                RoutePictureBase64 = t.RoutePicture,
                PhotographBase64 = t.Photograph,
                Location = t.Location,
                Email = t.Email,
                TelNumber = t.TelNumber,
                Worktime = t.Worktime
            }).ToList();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> BlogReports()
        {
            var reports = await db.BlogReports
                .Include(r => r.Blog)
                .ThenInclude(b => b.Author)
                .Include(r => r.Reporter)
                .ToListAsync();

            var model = reports.Select(r => new BlogReportViewModel
            {
                Id = r.Id.Value,
                BlogId = r.BlogId,
                BlogTitle = r.Blog.Title,
                AuthorName = r.Blog.Author.UserName,
                ReporterName = r.Reporter.UserName,
                ReportedOn = r.ReportedOn
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            var blog = await db.Blogs.FindAsync(id);
            if (blog != null)
            {
                db.Blogs.Remove(blog);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("BlogReports", "Admin");
        }


        [HttpPost]
        public async Task<IActionResult> DeleteReport(int id)
        {
            var report = await db.BlogReports.FindAsync(id);
            if (report != null)
            {
                db.BlogReports.Remove(report);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("BlogReports", "Admin");
        }


        [HttpPost]
        public async Task<IActionResult> BlockUser(string userId)
        {
            var userExists = await db.BlockedUsers.AnyAsync(x => x.UserId == userId);
            if (!userExists)
            {
                var blocked = new BlockedUser
                {
                    UserId = userId,
                    BlockedOn = DateTime.Now,
                    
                };

                await db.BlockedUsers.AddAsync(blocked);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("BlockedUsers", "Admin");
        }

        [HttpPost]
        public async Task<IActionResult> UnblockUser(int id)
        {
            var user = await db.BlockedUsers.FindAsync(id);
            if (user != null)
            {
                db.BlockedUsers.Remove(user);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("BlockedUsers", "Admin");
        }

        [HttpGet]
        public async Task<IActionResult> BlockedUsers()
        {
            var users = await db.BlockedUsers
                .Include(x => x.User)
                .ToListAsync();

            var model = users.Select(x => new BlockedUserViewModel(
             x.Id,  
             x.User.UserName,
             x.BlockedOn
            )).ToList();

            return View(model);
        }
    }
}
