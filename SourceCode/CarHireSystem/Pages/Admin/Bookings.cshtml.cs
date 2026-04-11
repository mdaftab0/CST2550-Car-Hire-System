using CarHireSystem.Database;
using CarHireSystem.DataStructures;
using CarHireSystem.Models;
using CarHireSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CarHireSystem.Pages.Admin;

[Authorize(Roles = "Admin")]
public class BookingsModel : PageModel
{
    private readonly CarHireDbContext _db;
    private readonly BookingService _bookingService;
    private readonly BinarySearchTree _bst;

    public BookingsModel(CarHireDbContext db, BookingService bookingService, BinarySearchTree bst)
    {
        _db = db;
        _bookingService = bookingService;
        _bst = bst;
    }

    public Booking[] Bookings { get; set; } = Array.Empty<Booking>();
    public Car[] Cars { get; set; } = Array.Empty<Car>();
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }

    // Cancel
    [BindProperty] public int CancelBookingId { get; set; }

    // Edit
    [BindProperty] public int    EditBookingId   { get; set; }
    [BindProperty] public string EditCustomerName  { get; set; } = "";
    [BindProperty] public string EditCustomerPhone { get; set; } = "";
    [BindProperty] public DateTime EditStartDate  { get; set; }
    [BindProperty] public DateTime EditEndDate    { get; set; }

    public async Task OnGetAsync()
    {
        await LoadAsync();
    }

    public async Task<IActionResult> OnPostCancelAsync()
    {
        var booking = await _db.Bookings.FindAsync(CancelBookingId);
        if (booking == null)
        {
            ErrorMessage = "Booking not found.";
            await LoadAsync();
            return Page();
        }

        booking.IsActive    = false;
        booking.IsCancelled = true;

        // Free the car in DB
        var dbCar = await _db.Cars.FindAsync(booking.CarID);
        if (dbCar != null)
            dbCar.IsAvailable = true;

        await _db.SaveChangesAsync();

        // Sync HashTable
        var htBooking = _bookingService.GetBooking(CancelBookingId);
        if (htBooking != null)
        {
            htBooking.IsActive    = false;
            htBooking.IsCancelled = true;
        }

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

    public async Task<IActionResult> OnPostEditAsync()
    {
        if (EditEndDate <= EditStartDate)
        {
            ErrorMessage = "End date must be after the start date.";
            await LoadAsync();
            return Page();
        }

        var booking = await _db.Bookings.FindAsync(EditBookingId);

        if (booking == null)
        {
            ErrorMessage = "Booking not found.";
            await LoadAsync();
            return Page();
        }

        var car = await _db.Cars.FindAsync(booking.CarID);
        decimal pricePerDay = car?.PricePerDay ?? 0;

        booking.UpdateTerms(EditStartDate, EditEndDate, EditCustomerName, EditCustomerPhone, pricePerDay);
        await _db.SaveChangesAsync();

        // Sync HashTable
        var htBooking = _bookingService.GetBooking(EditBookingId);
        if (htBooking != null)
            htBooking.UpdateTerms(EditStartDate, EditEndDate, EditCustomerName, EditCustomerPhone, pricePerDay);

        SuccessMessage = $"Booking #{EditBookingId} has been updated.";
        await LoadAsync();
        return Page();
    }

    private async Task LoadAsync()
    {
        Bookings = await _db.Bookings.OrderByDescending(b => b.BookingID).ToArrayAsync();
        Cars     = await _db.Cars.ToArrayAsync();
    }
}
