using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project.Entities;
using System.Data;

namespace Project.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }   

        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Relationship> Relationships { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Relationship>()
                .HasOne(x => x.Follower)
                .WithMany(x => x.Followers)
                .HasForeignKey(x => x.FollowerId);

            builder.Entity<Relationship>()
                .HasOne(x => x.Following)
                .WithMany(x => x.Followings)
                .HasForeignKey(x => x.FollowingId);
        }

    }
}
