using WorldKartIdentity.Database;

namespace WorldKartIdentity.ViewModel
{
    public class TrackTrajectoryViewModel
    {
        public int Id { get; set; }

        public int TrackId { get; set; }

        public string UserId { get; set; } = string.Empty;

        public string Base64 { get; set; } = null!;

        public DateTime CreatedOn { get; set; }

        public User User { get; set; } = null!;

        public Track? Track { get; set; }

        public static TrackTrajectoryViewModel TrajectoryToTrajectoryVM(TrackTrajectory trajectory)
        {
            return new TrackTrajectoryViewModel
            {
                Id = trajectory.Id,
                TrackId = trajectory.TrackId,
                UserId = trajectory.UserId,
                Base64 = trajectory.TrajectoryBase64,
                Track = trajectory.Track,
                User = trajectory.User,
                CreatedOn = trajectory.CreatedOn
            };
        }

    }
}
