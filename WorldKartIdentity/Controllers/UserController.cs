using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly UserManager<User> _users;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;
        public UserController(UserManager<User> users, SignInManager<User> signInManager, IConfiguration config, ApplicationDbContext db)
        {
            _users = users;
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
                return View(userVM);

            bool emailExists = await _db.Users.AnyAsync(u => u.Email == userVM.Email);
            if(emailExists)
            {
                ModelState.AddModelError("Email", "Имейлът вече е регистриран!");
                return View();
            }

            User user = UserViewModel.UserVMToUser(userVM);
            var result = await _users.CreateAsync(user);//за Юсър

            if (result.Succeeded)
            {
                    await _users.AddToRoleAsync(user, "Users");//даване на роля като Юсър //грешката идва от тук
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
                return View(userVM);
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
            var user = await _users.FindByEmailAsync(userVM.Email!);
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
        public IActionResult UserProfile()
        {
            return View();
        }
    }
}
