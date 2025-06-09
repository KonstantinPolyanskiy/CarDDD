using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CarDDD.Infrastructure.Contexts;

public class ApplicationDbContextFactory
    : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=db;Port=5313;Database=main_db;Username=admin;Password=password",
                npg => npg.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
            .Options;

        return new ApplicationDbContext(options);
    }
}
