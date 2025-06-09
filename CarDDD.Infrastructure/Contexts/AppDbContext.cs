using CarDDD.ApplicationServices.Models.StorageModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarDDD.Infrastructure.Contexts;

public class ApplicationDbContext : DbContext
{
    public DbSet<CarSnapshot> Cars { get; set; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
    { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<CarSnapshot>(e =>
        {
            e.HasKey(c => c.Id);
            
            e.Property(c => c.Id).ValueGeneratedNever();
        });
    }
}