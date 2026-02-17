using Microsoft.EntityFrameworkCore;

namespace RoyalVilla_API.Data;

public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{
   public DbSet<Villa> Villas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Villas>().HasData();
    }
}
