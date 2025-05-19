using Microsoft.EntityFrameworkCore;

namespace WeatherApi.Models
{
    public class CityDbContext : DbContext
    {
        public CityDbContext(DbContextOptions<CityDbContext> options) : base(options)
        {
        }

        public DbSet<City> Cities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.HasIndex(c => c.Country);
                entity.HasIndex(c => c.Name);
                entity.OwnsOne(c => c.Coord);
                entity.Property(c => c.Country).HasMaxLength(100);
                entity.Property(c => c.Name).HasMaxLength(200);
            });
        }
    }
}
