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

        public string? PhoneNumber { get; set; }

        public UserViewModel()
        {
            UserName = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
            RepeatPassword = string.Empty;
            Picture = string.Empty;
            PhoneNumber = string.Empty;
        }

        public UserViewModel(User user)
        {
            UserName = user.UserName;
            Email = user.Email;
            Password = user.PasswordHash;
            Picture = user.Picture;
            PhoneNumber = user.PhoneNumber;
        }

        public static User UserVMToUser(UserViewModel userVM)
        {
            User user = new User();
            user.UserName = userVM.UserName;
            user.Email = userVM.Email;
            user.PasswordHash = userVM.Password;
            user.Picture = userVM.Picture;
            user.PhoneNumber = userVM.PhoneNumber;
            return user;
        }
    }
}
