using CarDDD.Core.SnapshotModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarDDD.Infrastructure.Contexts;

public class ApplicationDbContext : IdentityDbContext
{
    public DbSet<CarSnapshot> Cars { get; set; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
    { }
}