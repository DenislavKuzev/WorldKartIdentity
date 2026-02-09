using Microsoft.AspNetCore.Identity;

namespace WorldKartIdentity.Database
{
    public class User : IdentityUser
    {
        public string? Picture { get; set; } = string.Empty;
        public string? Country { get; set; } 
        public string? Bio { get; set; }
        public string? RoleInKarting { get; set; } // Пилот/Механик
        public string? FacebookUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? TikTokUrl { get; set; }
        public string? YoutubeUrl { get; set; }

        public ICollection<TrackAnnotation> TrackAnnotations { get; set; } = new List<TrackAnnotation>();


    }
}
