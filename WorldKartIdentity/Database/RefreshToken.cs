namespace WorldKartIdentity.Database
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string UserId { get; set; } = default!;
        public string TokenHash { get; set; } = default!; // store SHA256 of the token
        public DateTime ExpiresAtUtc { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? RevokedAtUtc { get; set; }
        public string? ReplacedByTokenHash { get; set; } // chain / rotation
        public string? Device { get; set; }
        public string? IpAddress { get; set; }


        public bool IsActive => RevokedAtUtc == null && DateTime.UtcNow < ExpiresAtUtc;
    }
}
