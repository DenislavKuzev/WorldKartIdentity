using brevo_csharp.Api;
using brevo_csharp.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OpenAI.Chat;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.ClientModel;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
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
            _signInManager.Options.Password.RequireNonAlphanumeric = false;
            _userManager.Options.Password.RequireNonAlphanumeric = false;
        }

        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Registration([FromBody] UserViewModel userVM)
        {
            if (!ModelState.IsValid)
                return Json(ModelState);

            User user = UserViewModel.UserVMToUser(userVM);
            var result = await _userManager.CreateAsync(user);//за Юсър

            var passwordResult = await _userManager.AddPasswordAsync(user, userVM.Password);//за правилно хеширане на паролата

            if (result.Succeeded && passwordResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Users");//даване на роля като Юсър 
                                                                 // Влизане веднага след регистрация
                await _signInManager.SignInAsync(user, isPersistent: false);
                return Json(new
                {
                    success = true
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = GetRegistrationErrorMessage(result)
                });
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ExternalLogin(string provider)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "User");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        public async Task <IActionResult> ExternalLoginCallback()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if(info != null)
            {
                var signInRes = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true);

                if (signInRes.Succeeded)
                {
                    await _signInManager.UpdateExternalAuthenticationTokensAsync(info);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    string email = info.Principal.FindFirstValue(ClaimTypes.Email);
                    string name = info.Principal.FindFirstValue(ClaimTypes.Name).Replace(" ", "_");
                    

                    var user = new User
                    {
                        UserName = name,
                        Email = email
                    };

                    var createRes = await _userManager.CreateAsync(user);
                    if (createRes.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Users");
                        var loginRes = await _userManager.AddLoginAsync(user, info);
                        if (loginRes.Succeeded)
                        {
                            await _signInManager.SignInAsync(user, isPersistent: true);
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    
                    return RedirectToAction("Login");
                }

                    
            }
            else
            {
                TempData["Message"] = $"Грешка при влизане с {info.LoginProvider}|F";
                return RedirectToAction("Login");
            }
            
        }

        


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Login(UserViewModel userVM)
        {
            var isBlocked = _db.BlockedUsers.Any(b => b.UserId == userVM.Id);
            if (isBlocked)
            {
                ModelState.AddModelError("Email", "Вашият акаунт е блокиран!");
                return Json(new
                {
                    success = false
                });
            }


            var user = await _userManager.FindByEmailAsync(userVM.Email!);
            if (user == null)
            {
                return Json(new
                {
                    success = false
                });
            }


            var result = await _signInManager.PasswordSignInAsync(
        user, userVM.Password!, isPersistent: false, lockoutOnFailure: false);

            return Json(new 
            {
                success = result.Succeeded
            });
        }

        [HttpGet]
        public async Task<IActionResult> UserProfile()
        {
            UserViewModel loggedUserVM = new UserViewModel();
            var likedBlogs = new List<BlogViewModel>();
            var likedTracks = new List<TrackViewModel>();

            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);
                User? loggedUser = await _db.Users.Include(t => t.TrackTrajectories).FirstOrDefaultAsync(bl => bl.Id == userId);
                
                List<BlogPost> blogs = await _db.BlogLikes.Where(bl => bl.UserId == userId).Include(b => b.Blog).ThenInclude(b => b.Author).Select(bl => bl.Blog).ToListAsync();
                likedBlogs = blogs.Select(b => new BlogViewModel(b)).ToList();

                List<Track> tracks = await _db.TrackLikes.Where(tl => tl.UserId == userId).Select(tl => tl.Track).ToListAsync();
                likedTracks = tracks.Select(t => new TrackViewModel(t)).ToList();

                if (loggedUser != null)
                {
                    loggedUserVM = new UserViewModel(loggedUser);
                    loggedUserVM.LikedBlogs = likedBlogs;
                    loggedUserVM.LikedTracks = likedTracks;
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
            user.UserName = userVM.UserName;
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
                string key = Environment.GetEnvironmentVariable("EMAIL_API_KEY");
                brevo_csharp.Client.Configuration.Default.ApiKey.Add("api-key", key);

                var api = new TransactionalEmailsApi();
                var emailToSend = new SendSmtpEmail(
                    sender: new SendSmtpEmailSender("World Karting Arena", "worldkarting101@gmail.com"),
                    to: [new SendSmtpEmailTo(email)],
                    subject: "Забравена парола",
                    htmlContent: $"<p>Здравейте,</p>\r\n\r\n<p>Получихме заявка за смяна на паролата към Вашия акаунт.</p>" +
                    $"\r\n\r\n<p>За да зададете нова парола, моля натиснете бутона по-долу:</p>\r\n\r\n<p style='text-align:center;margin:30px 0;'>\r\n" +
                    $"<a href='{resetLink}' \r\n   style='background-color:#DC3545;\r\n          color:#ffffff;\r\n          padding:12px 25px;\r\n          text-decoration:none;\r\n          border-radius:6px;\r\n " +
                    $"         font-weight:bold;\r\n          display:inline-block;'>\r\n    Смяна на парола\r\n</a>\r\n</p>\r\n\r\n" +
                    $"<p>Ако бутонът не работи, копирайте и поставете следния линк в браузъра си: {resetLink}</p>\r\n\r\n<p style='word-break:break-all;color:#2563eb;'>\r\n{resetLink}" +
                    $"\r\n</p>\r\n\r\n<p style='margin-top:25px;'>" +
                    $"\r\nАко Вие не сте заявили смяна на парола, можете спокойно да игнорирате този имейл.\r\n</p>");


                //string key = "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";
                //var client = new SendGridClient(key);
                //SendGridMessage msg = new SendGridMessage()
                //{
                //    From = new EmailAddress("worldkarting101@gmail.com"),
                //    Subject = "Забравена парола",
                //    HtmlContent = $"<p>Здравейте,</p>\r\n\r\n<p>Получихме заявка за смяна на паролата към Вашия акаунт.</p>\r\n\r\n<p>За да зададете нова парола, моля натиснете бутона по-долу:</p>\r\n\r\n<p style='text-align:center;margin:30px 0;'>\r\n<a href='{resetLink}' \r\n   style='background-color:#DC3545;\r\n          color:#ffffff;\r\n          padding:12px 25px;\r\n          text-decoration:none;\r\n          border-radius:6px;\r\n          font-weight:bold;\r\n          display:inline-block;'>\r\n    Смяна на парола\r\n</a>\r\n</p>\r\n\r\n<p>Ако бутонът не работи, копирайте и поставете следния линк в браузъра си:</p>\r\n\r\n<p style='word-break:break-all;color:#2563eb;'>\r\n{resetLink}\r\n</p>\r\n\r\n<p style='margin-top:25px;'>\r\nАко Вие не сте заявили смяна на парола, можете спокойно да игнорирате този имейл.\r\n</p>"
                //};

                //msg.AddTo(new EmailAddress(email));
                //var res = await client.SendEmailAsync(msg);
                var res = await api.SendTransacEmailAsyncWithHttpInfo(emailToSend);

                if (res.StatusCode == 201)
                {
                    return Json(new
                    {
                        type = "success",
                        msg = "Линк за смяна на парола беше изпратен на вашия имейл."
                    });
                }
                return Json(new
                    {
                        type = "error",
                        msg = $"Грешка при изпращане на имейла. Код на грешката: {res.StatusCode}"
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending email: " + $"{ex.Message}\n \n {ex.InnerException} \n \n {ex.HelpLink}");
                return Json(new
                {
                    type = "error",
                    msg = "Грешка при изпращане на имейла."
                });
            }

        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPasswordViewModel { Token = token, Email = email };
            return View(model);
        }

        [HttpPost("/user/resetpassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            string token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));

            var res = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
            if (res.Succeeded)
            {
                TempData["Message"] = "Паролата беше успешно сменена. Можете да влезете с новата си парола|S";
                return RedirectToAction("Index", "Home");

            }
            else
            {
                TempData["Message"] = "Грешка при смяна на паролата. Възможно е линкът да е изтекъл или да е невалиден|F";
                return RedirectToAction("Index", "Home");
            }

        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> Notifications()
        {
            var userId = _userManager.GetUserId(User);
            var notifications = await _db.Notifications.Include(n => n.User)
                .Where(n => n.UserId == userId || string.IsNullOrEmpty(userId)) // user specific or global notifications
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            List<NotificationViewModel> viewmodel = notifications.Select(n => new NotificationViewModel(n)).ToList();
            return View(viewmodel);
        }



        private string GetRegistrationErrorMessage(IdentityResult result)
        {
            var error = result.Errors.FirstOrDefault();
            string message = "Грешка при регистрация.";
            if (error.Code == "DuplicateEmail")
            {
                message = "Потребител с този имейл вече е регестриран.";//latin
            }
            else if (error.Code == "DuplicateUserName")
            {
                message = "Потребител с това име вече е регистриран.";
            }
            return message;
        }

        
    }
}
