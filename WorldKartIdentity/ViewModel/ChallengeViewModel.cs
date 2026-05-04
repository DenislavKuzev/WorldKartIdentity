namespace WorldKartIdentity.ViewModel
{
    public class ChallengeViewModel
    {
        public TrackTrajectoryViewModel ChallengerTrajectory { get; set; }

        public TrackTrajectoryViewModel ChallengedTrajectory { get; set; }

        public int challengerTrajId { get; set; }

        public int challengedTrajId { get; set; }

    }
}
