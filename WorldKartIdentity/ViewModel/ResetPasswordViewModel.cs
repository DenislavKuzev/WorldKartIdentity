namespace WorldKartIdentity.ViewModel
{
    public class ResetPasswordViewModel
    {
        public string Token { get; set; } = null!;
        public string Email { get; set; } = null!;

        public string NewPassword { get; set; } = string.Empty;
    }
}