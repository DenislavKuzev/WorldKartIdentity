using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost/*("registration")*/]
        public async Task<IActionResult> Registration(UserViewModel userVM)
        {
            if (!ModelState.IsValid)
                return View(userVM);

            if (userVM.Password != userVM.RepeatPassword)
            {
                ModelState.AddModelError("", "Паролите не съвпадат!");
                return View();
            }

            User user = UserViewModel.UserVMToUser(userVM);
            var result = await _users.CreateAsync(user, userVM.Password);//за Юсър

            if (result.Succeeded)
            {
                    await _users.AddToRoleAsync(user, "Users");//даване на роля като Юсър //грешката идва от тук
                    // Влизане веднага след регистрация
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Privacy", "Home", new UserViewModel(user));
            }
            else
            {
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
            }
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);
            return RedirectToAction("", "");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost/*("login")*/]
        public async Task<IActionResult> Login(UserViewModel userVM)
        {
            var user = await _users.FindByEmailAsync(userVM.Email);
            if (user == null)
                return Unauthorized();

            var pass = await _signInManager.CheckPasswordSignInAsync(user, userVM.Password, lockoutOnFailure: false);
            if (!pass.Succeeded)
                return Unauthorized();

            var token = GenerateJwt(user, await _users.GetRolesAsync(user));
            var familyId = Guid.NewGuid().ToString("N");
            var (refreshPlain, refreshHash) = GenerateRefreshToken();

            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = refreshHash,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(14), // choose duration
                Device = Request.Headers["User-Agent"].ToString(),
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            };
            _db.RefreshTokens.Add(refreshToken);
            await _db.SaveChangesAsync();
            //return Ok(new TokenPairResponse { AccessToken = token, RefreshToken = refreshPlain });
            return RedirectToAction("Privacy", "Home");
        }

        private string GenerateJwt(User user, IList<string> roles)
        {
            var jwt = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new(ClaimTypes.Name, user.UserName ?? user.Email ?? "")
        };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
