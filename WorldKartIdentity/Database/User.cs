using Microsoft.AspNetCore.Identity;

namespace WorldKartIdentity.Database
{
    public class User : IdentityUser
    {
        public string? Picture { get; set; } = string.Empty;
    }
}
