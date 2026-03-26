using Microsoft.EntityFrameworkCore;
using CarHireSystem.Models;

namespace CarHireSystem.Database;

public class CarHireDbContext : DbContext
{
    public CarHireDbContext(DbContextOptions<CarHireDbContext> options) : base(options)
    {
    }

    public DbSet<Car> Cars { get; set; }
    public DbSet<Booking> Bookings { get; set; }
}