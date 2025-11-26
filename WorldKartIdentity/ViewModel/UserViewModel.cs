using WorldKartIdentity.Database;

namespace WorldKartIdentity.ViewModel
{
    public class UserViewModel
    {
        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string RepeatPassword { get; set; } = string.Empty;

        public string? Picture { get; set; }

        public UserViewModel()
        {
            UserName = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
            RepeatPassword = string.Empty;
            Picture = string.Empty;
        }

        public UserViewModel(User user)
        {
            UserName = user.UserName;
            Email = user.Email;
            Password = user.PasswordHash;
            Picture = user.Picture;
        }

        public static User UserVMToUser(UserViewModel userVM)
        {
            User user = new User();
            user.UserName = userVM.UserName;
            user.Email = userVM.Email;
            user.PasswordHash = userVM.Password;
            user.Picture = userVM.Picture;
            return user;
        }
    }
}
