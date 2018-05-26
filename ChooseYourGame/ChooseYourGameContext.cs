using ChooseYourGame.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChooseYourGame
{
    public class ChooseYourGameContext : IdentityDbContext
    {
        public ChooseYourGameContext(DbContextOptions<ChooseYourGameContext> options) : base(options) { }

        public DbSet<Tag> Tags { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Commentary> Commentaries { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Follower> Followers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Tag>().HasIndex(t => t.Description).IsUnique();

            modelBuilder.Entity<IdentityUser>().HasIndex(u => u.Email).IsUnique();
            
            modelBuilder.Entity<BlogTag>().HasKey(t => new { t.BlogId, t.TagId });
            modelBuilder.Entity<BlogTag>().HasOne(bt => bt.Blog).WithMany(b => b.BlogTag).HasForeignKey(bt => bt.BlogId);
            modelBuilder.Entity<BlogTag>().HasOne(bt => bt.Tag).WithMany(t => t.BlogTag).HasForeignKey(bt => bt.TagId);            
        }
    }
}