using CarHireSystem.Database;
using CarHireSystem.DataStructures;
using CarHireSystem.Models;
using CarHireSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

    [BindProperty] public int BookingId { get; set; }

    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task OnPostAsync()
    {
        // Try HashTable first (fast, in-memory), fall back to DB on app restart
        Booking? booking = _bookingService.GetBooking(BookingId);
        if (booking == null)
            booking = await _db.Bookings.FindAsync(BookingId);

        if (booking == null)
        {
            ErrorMessage = "Booking not found.";
            return;
        }

        // Find car in BST for in-memory update and display name
        var results = _bst.SearchByPriceRange(0, decimal.MaxValue);
        Car? car = null;
        for (int i = 0; i < results.Count; i++)
        {
            if (results.Get(i).Id == booking.CarID)
            {
                car = results.Get(i);
                break;
            }
        }

        if (car == null)
        {
            ErrorMessage = "Car not found.";
            return;
        }

        // Update in-memory BST car
        car.IsAvailable = true;

        // Update in-memory HashTable booking if present
        var htBooking = _bookingService.GetBooking(BookingId);
        if (htBooking != null)
            htBooking.Booked = false;

        // Persist car availability to Azure SQL
        var dbCar = await _db.Cars.FindAsync(booking.CarID);
        if (dbCar != null)
            dbCar.IsAvailable = true;

        // Persist booking status to Azure SQL
        var dbBooking = await _db.Bookings.FindAsync(BookingId);
        if (dbBooking != null)
            dbBooking.Booked = false;

        await _db.SaveChangesAsync();

        SuccessMessage = $"Car returned successfully. {car.Make} {car.Model} is now available.";
    }
}
