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
public class MyBookingsModel : PageModel
{
    private readonly CarHireDbContext _db;
    private readonly BookingService _bookingService;
    private readonly BinarySearchTree _bst;

    public MyBookingsModel(CarHireDbContext db, BookingService bookingService, BinarySearchTree bst)
    {
        _db = db;
        _bookingService = bookingService;
        _bst = bst;
    }

    public Booking[] Bookings { get; set; } = Array.Empty<Booking>();
    public Car[] Cars { get; set; } = Array.Empty<Car>();
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }

    [BindProperty] public int CancelBookingId { get; set; }

    public async Task OnGetAsync()
    {
        await LoadAsync();
    }

    public async Task<IActionResult> OnPostCancelAsync()
    {
        var email = User.Identity!.Name!;

        var booking = await _db.Bookings.FindAsync(CancelBookingId);

        if (booking == null || booking.CustomerEmail != email)
        {
            ErrorMessage = "Booking not found.";
            await LoadAsync();
            return Page();
        }

        if (!booking.IsActive)
        {
            ErrorMessage = "This booking is already returned or cancelled.";
            await LoadAsync();
            return Page();
        }

        if (booking.StartDate.Date <= DateTime.UtcNow.Date)
        {
            ErrorMessage = "Bookings that have already started cannot be cancelled. Please use the Returns page.";
            await LoadAsync();
            return Page();
        }

        booking.IsActive = false;

        var dbCar = await _db.Cars.FindAsync(booking.CarID);
        if (dbCar != null)
            dbCar.IsAvailable = true;

        await _db.SaveChangesAsync();

        // Sync HashTable
        var htBooking = _bookingService.GetBooking(CancelBookingId);
        if (htBooking != null)
            htBooking.IsActive = false;

        // Sync BST car
        var bstCars = _bst.SearchByPriceRange(0, decimal.MaxValue);
        for (int i = 0; i < bstCars.Count; i++)
        {
            if (bstCars[i].Id == booking.CarID)
            {
                bstCars[i].IsAvailable = true;
                break;
            }
        }

        SuccessMessage = $"Booking #{CancelBookingId} has been cancelled.";
        await LoadAsync();
        return Page();
    }

    private async Task LoadAsync()
    {
        var email = User.Identity!.Name!;

        Bookings = await _db.Bookings
            .Where(b => b.CustomerEmail == email)
            .OrderByDescending(b => b.BookingID)
            .ToArrayAsync();

        var carIds = Bookings.Select(b => b.CarID).Distinct().ToArray();
        Cars = await _db.Cars
            .Where(c => carIds.Contains(c.Id))
            .ToArrayAsync();
    }
}
