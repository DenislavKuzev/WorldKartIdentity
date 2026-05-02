using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace WorldKartIdentity.Database
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> Options) : base(Options) { }
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<TrackRequest> TrackRequests { get; set; }
        public DbSet<Track> Tracks { get; set; }
        public DbSet<BlogPost> Blogs { get; set; }
        public DbSet<TrackLike> TrackLikes { get; set; }

        public DbSet<BlogLikes> BlogLikes { get; set; }

        public DbSet<BlogReport> BlogReports { get; set; }
        public DbSet<BlockedUser> BlockedUsers { get; set; }

        public DbSet<TrackAnnotation> TrackAnnotations { get; set; }

        public DbSet<TrackTrajectory> TrackTrajectories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().ToTable("Users");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<Track>().ToTable("Tracks");
            builder.Entity<BlogPost>().ToTable("BlogPosts");
            builder.Entity<BlogReport>().ToTable("BlogReports");
            builder.Entity<TrackRequest>().ToTable("TrackRequests");
            builder.Entity<BlockedUser>().ToTable("BlockedUsers");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
            builder.Entity<RefreshToken>().ToTable("RefreshTokens");
            builder.Entity<TrackAnnotation>().ToTable("TrackAnnotations");
            builder.Entity<TrackTrajectory>().ToTable("TrackTrajectory");

            builder.Entity<User>()
.HasMany(u => u.TrackAnnotations)
.WithOne(ta => ta.User)
.HasForeignKey(ta => ta.UserId)
.OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TrackLike>()
            .HasKey(x => new { x.UserId, x.TrackId });
            builder.Entity<BlogLikes>().HasKey(x => x.Id);
            builder.Entity<TrackAnnotation>().HasKey(x => x.Id);
            builder.Entity<TrackTrajectory>().HasKey(x => x.Id);

            builder.Entity<TrackLike>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TrackLike>()
                .HasOne(x => x.Track)
                .WithMany(t => t.Likes)
                .HasForeignKey(x => x.TrackId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TrackAnnotation>()
    .HasOne(x => x.Track)
    .WithMany(x => x.Annotations)
    .HasForeignKey(x => x.TrackId)
    .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TrackTrajectory>()
                .HasOne(x => x.Track)
                .WithMany(t => t.Trajectories)
                .HasForeignKey(x => x.TrackId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<BlogLikes>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<BlogLikes>()
                .HasOne(x => x.Blog)
                .WithMany(b => b.BlogLikes)
                .HasForeignKey(x => x.BlogId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
