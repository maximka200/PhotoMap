using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PhotoMapAPI.Models;

namespace PhotoMapAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<string>, string>
    {
        public DbSet<Point> Points { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Avatar> Avatars { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 
            
            modelBuilder.Entity<User>()
                .HasOne(u => u.UserAvatar)
                .WithOne(a => a.User)
                .HasForeignKey<Avatar>(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);  

            
            modelBuilder.Entity<Photo>()
                .HasOne(p => p.Point)
                .WithMany(p => p.Photos)
                .HasForeignKey(p => p.PointId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}