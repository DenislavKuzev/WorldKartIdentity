using System.ComponentModel.DataAnnotations.Schema;

namespace WorldKartIdentity.Database
{
    public class TrackAnnotation
    {
        public int Id { get; set; }

        public int TrackId { get; set; }

        public string AnnotationJson { get; set; } = null!;

        public string UserId { get; set; } = default!;

        public string? UserAuthData { get; set; }

        public Track Track { get; set; } = null!;

        public User User { get; set; } = default!;
    }
}
