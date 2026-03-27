using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CarHireSystem.Database;

public class CarHireDbContextFactory : IDesignTimeDbContextFactory<CarHireDbContext>
{
    public CarHireDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<CarHireDbContext>();
        optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));

        return new CarHireDbContext(optionsBuilder.Options);
    }
}
