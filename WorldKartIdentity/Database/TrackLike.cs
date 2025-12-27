namespace WorldKartIdentity.Database
{
    public class TrackLike
    {
        public string? UserId { get; set; }
        public User? User { get; set; }

        public int TrackId { get; set; }
        public Track? Track { get; set; }
    }
}
