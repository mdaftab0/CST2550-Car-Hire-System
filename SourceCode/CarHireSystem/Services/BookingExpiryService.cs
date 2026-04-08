using CarHireSystem.Database;
using CarHireSystem.DataStructures;
using Microsoft.EntityFrameworkCore;

namespace CarHireSystem.Services;

public class BookingExpiryService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly BinarySearchTree _bst;
    private readonly HashTable _hashTable;
    private readonly ILogger<BookingExpiryService> _logger;

    public BookingExpiryService(
        IServiceProvider services,
        BinarySearchTree bst,
        HashTable hashTable,
        ILogger<BookingExpiryService> logger)
    {
        _services = services;
        _bst = bst;
        _hashTable = hashTable;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Run immediately on startup, then repeat every hour
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessExpiredBookingsAsync();
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    private async Task ProcessExpiredBookingsAsync()
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CarHireDbContext>();

        var today = DateTime.Today;

        var expired = await db.Bookings
            .Where(b => b.Booked && b.EndDate < today)
            .ToArrayAsync();

        if (expired.Length == 0) return;

        var carIds = expired.Select(b => b.CarID).Distinct().ToArray();
        var dbCars = await db.Cars
            .Where(c => carIds.Contains(c.Id))
            .ToArrayAsync();

        // Grab all BST cars once for in-memory sync
        var bstCars = _bst.SearchByPriceRange(0, decimal.MaxValue);

        foreach (var booking in expired)
        {
            // Mark booking returned in DB
            booking.Booked = false;

            // Mark car available in DB
            var dbCar = Array.Find(dbCars, c => c.Id == booking.CarID);
            if (dbCar != null)
                dbCar.IsAvailable = true;

            // Sync HashTable
            var htBooking = _hashTable.GetById(booking.BookingID);
            if (htBooking != null)
                htBooking.Booked = false;

            // Sync BST
            for (int i = 0; i < bstCars.Count; i++)
            {
                if (bstCars.Get(i).Id == booking.CarID)
                {
                    bstCars.Get(i).IsAvailable = true;
                    break;
                }
            }

            _logger.LogInformation(
                "Booking #{BookingId} auto-returned — Car #{CarId} is now available.",
                booking.BookingID, booking.CarID);
        }

        await db.SaveChangesAsync();
    }
}
