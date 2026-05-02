using WorldKartIdentity.Database;

namespace WorldKartIdentity.ViewModel
{
    public class NotificationViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string Message { get; set; } = null!;

        public NotificationType Type { get; set; }

        public DateTime CreatedAt { get; set; }

        public User? User { get; set; }


        public NotificationViewModel()
        {

        }

        public NotificationViewModel(Notification notification)
        {
            Title = notification.Title;
            Message = notification.Message;
            Type = notification.Type;
            CreatedAt = notification.CreatedAt;
            User = notification.User;
        }
    }
}
