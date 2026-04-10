using CarHireSystem.Database;
using CarHireSystem.DataStructures;
using CarHireSystem.Models;
using CarHireSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CarHireSystem.Pages;

[Authorize]
public class ReturnsModel : PageModel
{
    private readonly BookingService _bookingService;
    private readonly BinarySearchTree _bst;
    private readonly CarHireDbContext _db;

    public ReturnsModel(BookingService bookingService, BinarySearchTree bst, CarHireDbContext db)
    {
        _bookingService = bookingService;
        _bst = bst;
        _db = db;
    }

    public Booking[] ActiveBookings { get; set; } = Array.Empty<Booking>();
    public Car[] Cars { get; set; } = Array.Empty<Car>();
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        await LoadActiveBookingsAsync();
    }

    public async Task OnPostAsync(int bookingId)
    {
        var email = User.Identity!.Name!;

        // Load booking and verify it belongs to this user
        var booking = await _db.Bookings.FindAsync(bookingId);

        if (booking == null || booking.CustomerEmail != email)
        {
            ErrorMessage = "Booking not found.";
            await LoadActiveBookingsAsync();
            return;
        }

        if (!booking.IsActive)
        {
            ErrorMessage = "This car has already been returned.";
            await LoadActiveBookingsAsync();
            return;
        }

        // Find car in BST for in-memory update
        var results = _bst.SearchByPriceRange(0, decimal.MaxValue);
        Car? car = null;
        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].Id == booking.CarID)
            {
                car = results[i];
                break;
            }
        }

        if (car != null)
            car.IsAvailable = true;

        // Update HashTable booking if present
        var htBooking = _bookingService.GetBooking(bookingId);
        if (htBooking != null)
            htBooking.IsActive = false;

        // Persist to DB
        var dbCar = await _db.Cars.FindAsync(booking.CarID);
        if (dbCar != null)
            dbCar.IsAvailable = true;

        booking.IsActive = false;
        await _db.SaveChangesAsync();

        SuccessMessage = $"Return successful. {dbCar?.Make} {dbCar?.Model} is now back in the fleet.";
        await LoadActiveBookingsAsync();
    }

    private async Task LoadActiveBookingsAsync()
    {
        var email = User.Identity!.Name!;

        ActiveBookings = await _db.Bookings
            .Where(b => b.CustomerEmail == email && b.IsActive)
            .OrderByDescending(b => b.BookingID)
            .ToArrayAsync();

        var carIds = ActiveBookings.Select(b => b.CarID).Distinct().ToArray();
        Cars = await _db.Cars
            .Where(c => carIds.Contains(c.Id))
            .ToArrayAsync();
    }
}
