namespace WorldKartIdentity.Database
{
    public class Notification
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string Message { get; set; } = null!;

        public NotificationType Type { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? UserId { get; set; }

        public User? User { get; set; }

    }

    public enum NotificationType
    {
        NewTrack,        // broadcast
        //UpcomingEvent,   // broadcast // these 2 might be implemented in the future
        //EventEnded,      // broadcast
        RequestApproved, // targeted
        NewComment,      // targeted
        NewLike          // targeted
    }
}
