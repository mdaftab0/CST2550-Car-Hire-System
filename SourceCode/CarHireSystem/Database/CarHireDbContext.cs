using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CarHireSystem.Models;

namespace CarHireSystem.Database;

public class CarHireDbContext : IdentityDbContext<ApplicationUser>
{
    public CarHireDbContext(DbContextOptions<CarHireDbContext> options) : base(options)
    {
    }

    public DbSet<Car> Cars { get; set; }
    public DbSet<Booking> Bookings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Car>()
            .Property(c => c.Id)
            .ValueGeneratedNever();

        modelBuilder.Entity<Booking>()
            .Property(b => b.BookingID)
            .ValueGeneratedNever();
    }
}
