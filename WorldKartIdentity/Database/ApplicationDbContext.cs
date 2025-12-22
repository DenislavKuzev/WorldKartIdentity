using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WorldKartIdentity.Database
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> Options) : base(Options) { }
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        public DbSet<TrackRequest> TrackRequests {  get; set; }
        public DbSet<Track> Tracks { get; set; }

        public DbSet<BlogPost> Blogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().ToTable("Users");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<Track>().ToTable("Tracks");
            builder.Entity<BlogPost>().ToTable("BlogPosts");
            builder.Entity<TrackRequest>().ToTable("TrackRequests");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
            builder.Entity<RefreshToken>().ToTable("RefreshTokens");
        }
    }
}
