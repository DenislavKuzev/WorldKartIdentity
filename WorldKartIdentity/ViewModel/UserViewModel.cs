using System.ComponentModel.DataAnnotations;
using WorldKartIdentity.Database;

namespace WorldKartIdentity.ViewModel
{
    public class UserViewModel
    {
        [RegularExpression(@"^[a-zA-Z0-9_]{3,20}$")]
        public string? UserName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string? Email { get; set; } = string.Empty;

        [Required, MinLength(8)]
        public string? Password { get; set; } = string.Empty;

        [Compare("Password")]
        public string RepeatPassword { get; set; } = string.Empty;

        public string? Picture { get; set; }
        public string? Country { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Bio { get; set; }
        public string? RoleInKarting { get; set; } // Пилот/Механик
        public string? FacebookUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? TikTokUrl { get; set; }
        public string? YoutubeUrl { get; set; }


        public UserViewModel()
        {
            UserName = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
            RepeatPassword = string.Empty;
            Picture = string.Empty;
            PhoneNumber = string.Empty;
            Bio = string.Empty;
            Country = string.Empty;
            RoleInKarting = string.Empty;
            FacebookUrl = string.Empty;
            InstagramUrl = string.Empty;
            TikTokUrl = string.Empty;
            YoutubeUrl = string.Empty;
        }

        public UserViewModel(User user)
        {
            UserName = user.UserName;
            Email = user.Email;
            Password = user.PasswordHash;
            Picture = user.Picture;
            PhoneNumber = user.PhoneNumber;
            Bio = user.Bio;
            Country = user.Country;
            RoleInKarting = user.RoleInKarting;
            FacebookUrl = user.FacebookUrl;
            InstagramUrl = user.InstagramUrl;
            TikTokUrl = user.TikTokUrl;
            YoutubeUrl = user.YoutubeUrl;
        }
        public static User UserVMToUser(UserViewModel userVM)
        {
            User user = new User();
            user.UserName = userVM.UserName;
            user.Email = userVM.Email;
            //user.PasswordHash = userVM.Password;
            user.Picture = userVM.Picture;
            user.PhoneNumber = userVM.PhoneNumber;
            user.Bio = userVM.Bio;
            user.Country = userVM.Country;
            user.RoleInKarting = userVM.RoleInKarting;
            user.FacebookUrl = userVM.FacebookUrl;
            user.InstagramUrl = userVM.InstagramUrl;
            user.TikTokUrl = userVM.TikTokUrl;
            user.YoutubeUrl = userVM.YoutubeUrl;
            return user;
        }
    }
}
