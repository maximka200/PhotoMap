using Microsoft.EntityFrameworkCore;
using PhotoMapAPI.Models;

namespace PhotoMapAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Point> Points { get; set; }
        public DbSet<Photo> Photos { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Photo>()
                .HasOne(p => p.Point)
                .WithMany(p => p.Photos)
                .HasForeignKey(p => p.PointId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}