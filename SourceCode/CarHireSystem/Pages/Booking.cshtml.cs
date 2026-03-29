using CarHireSystem.Database;
using CarHireSystem.DataStructures;
using CarHireSystem.Models;
using CarHireSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarHireSystem.Pages;

[Authorize]
public class BookingModel : PageModel
{
    private readonly BookingService _bookingService;
    private readonly BinarySearchTree _bst;
    private readonly CarHireDbContext _db;

    public BookingModel(BookingService bookingService, BinarySearchTree bst, CarHireDbContext db)
    {
        _bookingService = bookingService;
        _bst = bst;
        _db = db;
    }

    [BindProperty] public int CarId { get; set; }
    [BindProperty] public string CustomerName { get; set; } = "";
    [BindProperty] public string CustomerEmail { get; set; } = "";
    [BindProperty] public string CustomerPhone { get; set; } = "";
    [BindProperty] public DateTime StartDate { get; set; }
    [BindProperty] public DateTime EndDate { get; set; }

    public Car? SelectedCar { get; set; }
    public Booking? Confirmation { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync(int? carId)
    {
        if (carId.HasValue)
        {
            CarId = carId.Value;
            SelectedCar = await _db.Cars.FindAsync(carId.Value);
        }
    }

    public async Task OnPostAsync()
    {
        // Re-load car for display in case of error
        SelectedCar = await _db.Cars.FindAsync(CarId);

        // Find car in BST
        var results = _bst.SearchByPriceRange(0, decimal.MaxValue);
        Car? car = null;
        for (int i = 0; i < results.Count; i++)
        {
            if (results.Get(i).Id == CarId)
            {
                car = results.Get(i);
                break;
            }
        }

        if (car == null)
        {
            ErrorMessage = "Car not found. Please go back and select a car from the search page.";
            return;
        }

        if (!car.IsAvailable)
        {
            ErrorMessage = "This car is no longer available. Please go back and choose another.";
            return;
        }

        if (EndDate <= StartDate)
        {
            ErrorMessage = "End date must be after the start date.";
            return;
        }

        var booking = new Booking(
            new Random().Next(1000, 9999),
            car.Id,
            CustomerName,
            CustomerEmail,
            CustomerPhone,
            StartDate,
            EndDate,
            car.PricePerDay
        );

        car.IsAvailable = false;
        _bookingService.CreateBooking(booking);

        _db.Bookings.Add(booking);
        var dbCar = await _db.Cars.FindAsync(car.Id);
        if (dbCar != null)
            dbCar.IsAvailable = false;

        await _db.SaveChangesAsync();

        Confirmation = booking;
    }
}
