using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WorldKartIdentity.Database;
using WorldKartIdentity.ViewModel;
using static WorldKartIdentity.Helpers.TokenHelper;

namespace WorldKartIdentity.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;
        public UserController(UserManager<User> users, SignInManager<User> signInManager, IConfiguration config, ApplicationDbContext db)
        {
            _userManager = users;
            _signInManager = signInManager;
            _config = config;
            _db = db;
        }

        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registration(UserViewModel userVM)
        {
            if (!ModelState.IsValid)
                return Json(ModelState);

            bool emailExists = await _db.Users.AnyAsync(u => u.Email == userVM.Email);
            if(emailExists)
            {
                return Json(new
                {
                    success = false,
                    message = "Имейлът вече е регистриран"
                });
            }

            User user = UserViewModel.UserVMToUser(userVM);
            var result = await _userManager.CreateAsync(user);//за Юсър
            await _userManager.AddPasswordAsync(user, userVM.Password);//за паролата

            if (result.Succeeded)
            {
                    await _userManager.AddToRoleAsync(user, "Users");//даване на роля като Юсър //грешката идва от тук
                    // Влизане веднага след регистрация
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home", new UserViewModel(user));
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return Json(ModelState);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost/*("login")*/]
        public async Task<IActionResult> Login(UserViewModel userVM)
        {
            var user = await _userManager.FindByEmailAsync(userVM.Email!);
            if (user == null)
            {
                ModelState.AddModelError("Email", "Невалиден имейл или парола.");
                return View(userVM);
            }


            var result = await _signInManager.PasswordSignInAsync(
        user, userVM.Password!, isPersistent: false, lockoutOnFailure: false);


            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Невалиден имейл или парола.");
                return View(userVM);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> UserProfile()
        {
            UserViewModel loggedUserVM = new UserViewModel();
            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);
                User? loggedUser = await _db.Users.FirstOrDefaultAsync(bl => bl.Id == userId);
                if (loggedUser != null)
                {
                    loggedUserVM = new UserViewModel(loggedUser);
                }
            }

            return View(loggedUserVM);
        }
        [HttpGet]
        public IActionResult EditUserProfile()
        {
            UserViewModel loggedUserVM = new UserViewModel();
            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);
                User? loggedUser = _db.Users.FirstOrDefault(bl => bl.Id == userId);
                if (loggedUser != null)
                {
                    loggedUserVM = new UserViewModel(loggedUser);
                }
            }
            return View(loggedUserVM);
        }
        [HttpPost]
        public async Task<IActionResult> EditUserProfile(UserViewModel userVM)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _db.Users.FindAsync(userId);
            if (user == null)
                return NotFound();
            //var editedUser = UserViewModel.UserVMToUser(userVM);
            //editedUser.Id = userId;
            //_db.Users.Update(editedUser);
            user.PhoneNumber = userVM.PhoneNumber;
            user.Bio = userVM.Bio;
            user.Country = userVM.Country;
            user.RoleInKarting = userVM.RoleInKarting;
            user.FacebookUrl = userVM.FacebookUrl;
            user.InstagramUrl = userVM.InstagramUrl;
            user.TikTokUrl = userVM.TikTokUrl;
            user.YoutubeUrl = userVM.YoutubeUrl;
            user.Picture = userVM.Picture;

            if (userVM.PictureFile != null && userVM.PictureFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/users");
                Directory.CreateDirectory(uploadsFolder);
                var fileName = Guid.NewGuid() + Path.GetExtension(userVM.PictureFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                { 
                    await userVM.PictureFile.CopyToAsync(stream);
                }
                user.Picture = "/img/users/" + fileName;
            }

            await _db.SaveChangesAsync();
            return RedirectToAction("UserProfile");
        }

        [HttpGet]
        public IActionResult UserPublicProfile(bool edit = false)
        {
            UserViewModel loggedUserVM = new UserViewModel();

            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);
                User? loggedUser = _db.Users.FirstOrDefault(bl => bl.Id == userId);
                if (loggedUser != null)
                {
                    loggedUserVM = new UserViewModel(loggedUser);
                }
            }
            return View(loggedUserVM);
        }
    }
}
