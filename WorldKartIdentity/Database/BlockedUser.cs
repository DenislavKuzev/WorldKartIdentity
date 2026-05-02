namespace WorldKartIdentity.Database
{
    public class BlockedUser
    {
        public int Id { get; set; }

        public string? UserId { get; set; }
        public User User { get; set; }
        public DateTime BlockedOn { get; set; } = DateTime.Now;
    }
}
