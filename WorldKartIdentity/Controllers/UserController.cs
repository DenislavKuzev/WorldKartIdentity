using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
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
            if (emailExists)
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

        [HttpGet("/user/me")]
        public async Task<JsonResult> Me()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return Json(new
                {
                    authenticated = false,
                });

            return Json(new
            {
                authenticated = true,
                userId = user.Id,
                username = user.UserName
            });
        }

        [HttpPost]
        public async Task<JsonResult> ForgotPassword([FromBody] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Json(new
                {
                    type = "error",
                    msg = "Потребител с този имейл не съществува"
                });
            }
            string pstoken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var tokenBytes = Encoding.UTF8.GetBytes(pstoken);
            var tokenEncoded = WebEncoders.Base64UrlEncode(tokenBytes);

       
            string resetLink = Url.Action("ResetPassword", "User", new { token = tokenEncoded, email = user.Email }, Request.Scheme) ?? string.Empty;
            
            try
            {
                // Credentials
                var credentials = new NetworkCredential("worldkarting101@gmail.com", Environment.GetEnvironmentVariable("EMAIL_APP_PASSWORD"));

                var mail = new MailMessage()
                {
                    From = new MailAddress(email, "World Karting Track"),
                    Subject = "Забравена парола",
                    Body = $"<p>Здравейте,</p>\r\n\r\n<p>Получихме заявка за смяна на паролата към Вашия акаунт.</p>\r\n\r\n<p>За да зададете нова парола, моля натиснете бутона по-долу:</p>\r\n\r\n<p style='text-align:center;margin:30px 0;'>\r\n<a href='{resetLink}' \r\n   style='background-color:#DC3545;\r\n          color:#ffffff;\r\n          padding:12px 25px;\r\n          text-decoration:none;\r\n          border-radius:6px;\r\n          font-weight:bold;\r\n          display:inline-block;'>\r\n    Смяна на парола\r\n</a>\r\n</p>\r\n\r\n<p>Ако бутонът не работи, копирайте и поставете следния линк в браузъра си:</p>\r\n\r\n<p style='word-break:break-all;color:#2563eb;'>\r\n{resetLink}\r\n</p>\r\n\r\n<p style='margin-top:25px;'>\r\nАко Вие не сте заявили смяна на парола, можете спокойно да игнорирате този имейл.\r\n</p>"

                };

                mail.IsBodyHtml = true;
                mail.To.Add(new MailAddress(email));


                var client = new SmtpClient()
                {
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = "smtp.gmail.com",
                    EnableSsl = true,
                    Credentials = credentials,
                    Timeout = 15000,
                };
                await client.SendMailAsync(mail);

                return Json(new
                {
                    type = "success",
                    msg = "Имейл за нулиране на парола бе изпратен на" + email
                });
            }
            catch (Exception)
            {
                return Json(new
                {
                    type = "error",
                    msg = "Имейлът не съществува"
                });
            }

        }

        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPasswordViewModel { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            string token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));

            var res = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
            if (res.Succeeded) 
            {
                TempData["Message"] = "Паролата беше успешно сменена. Можете да влезете с новата си парола-S";
                return Ok();
            }
            else
            {
                if(i)
            }
            return Ok();

        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
