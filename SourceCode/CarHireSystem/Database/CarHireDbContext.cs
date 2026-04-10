using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CarHireSystem.Models;

namespace CarHireSystem.Database;

public class CarHireDbContext : IdentityDbContext<ApplicationUser>
{
    public CarHireDbContext(DbContextOptions<CarHireDbContext> options) : base(options)
    {
    }

    public DbSet<Car> Cars => Set<Car>();
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Always call base first for Identity configuration
        base.OnModelCreating(modelBuilder);

        // Car Configuration
        modelBuilder.Entity<Car>(entity =>
        {
            entity.HasKey(c => c.Id);
            
            // Manual ID management (as requested)
            entity.Property(c => c.Id)
                  .ValueGeneratedNever();

            entity.Property(c => c.Make).IsRequired().HasMaxLength(50);
            entity.Property(c => c.Model).IsRequired().HasMaxLength(50);
            
            // Registration should be unique and indexed for fast searching
            entity.Property(c => c.Registration).IsRequired().HasMaxLength(20);
            entity.HasIndex(c => c.Registration).IsUnique();

            // Crucial: SQL Server needs to know the precision for decimal/money
            entity.Property(c => c.PricePerDay)
                  .HasPrecision(18, 2); 
        });

        // Booking Configuration
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(b => b.BookingID);
            
            entity.Property(b => b.BookingID)
                  .ValueGeneratedNever();

            entity.Property(b => b.CustomerName).IsRequired().HasMaxLength(100);
            entity.Property(b => b.CustomerEmail).IsRequired().HasMaxLength(150);
            entity.Property(b => b.CustomerPhone).HasMaxLength(20);

            // Map IsActive to the existing 'Booked' column — avoids a migration
            entity.Property(b => b.IsActive).HasColumnName("Booked");

            // Money precision
            entity.Property(b => b.TotalCost)
                  .HasPrecision(18, 2);

            // Relationship: A booking must have a car
            // This enforces "Referential Integrity" in the database
            entity.HasOne<Car>() 
                  .WithMany() 
                  .HasForeignKey(b => b.CarID)
                  .OnDelete(DeleteBehavior.Cascade);

            // Add an index to CarID because we search bookings by car often
            entity.HasIndex(b => b.CarID);
        });
    }
}
