namespace WorldKartIdentity.Database
{
    public class TrackTrajectory
    {
        public int Id { get; set; }
        public int TrackId { get; set; }

        public string UserId { get; set; } = null!;

        public string TrajectoryBase64 { get; set; } = null!;

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public Track Track { get; set; } = null!;

        public User User { get; set; } = default!;
    }
}
