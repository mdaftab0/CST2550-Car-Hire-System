using CarHireSystem.Database;
using CarHireSystem.DataStructures;
using CarHireSystem.Models;
using CarHireSystem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stripe;

namespace CarHireSystem.Pages;

public class BookingModel : PageModel
{
    private readonly BookingService _bookingService;
    private readonly BinarySearchTree _bst;
    private readonly CarHireDbContext _db;
    private readonly IConfiguration _config;
    private readonly UserManager<ApplicationUser> _userManager;

    public BookingModel(BookingService bookingService, BinarySearchTree bst, CarHireDbContext db,
        IConfiguration config, UserManager<ApplicationUser> userManager)
    {
        _bookingService = bookingService;
        _bst = bst;
        _db = db;
        _config = config;
        _userManager = userManager;
    }

    [BindProperty] public int CarId { get; set; }
    [BindProperty] public string CustomerName { get; set; } = "";
    [BindProperty] public string CustomerEmail { get; set; } = "";
    [BindProperty] public string CustomerPhone { get; set; } = "";
    [BindProperty] public DateTime StartDate { get; set; }
    [BindProperty] public DateTime EndDate { get; set; }
    [BindProperty] public string? PaymentIntentId { get; set; }

    public Car? SelectedCar { get; set; }
    public Booking? Confirmation { get; set; }
    public string? ErrorMessage { get; set; }
    public string StripePublishableKey { get; private set; } = "";

    // Pre-filled values for logged-in users
    public string PrefilledName { get; private set; } = "";
    public string PrefilledEmail { get; private set; } = "";
    public string PrefilledPhone { get; private set; } = "";
    public bool IsLoggedIn { get; private set; }

    public async Task OnGetAsync(int? carId)
    {
        StripePublishableKey = _config["Stripe:PublishableKey"] ?? "";

        if (carId.HasValue)
        {
            CarId = carId.Value;
            SelectedCar = await _db.Cars.FindAsync(carId.Value);
        }

        if (User.Identity?.IsAuthenticated == true)
        {
            IsLoggedIn = true;
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                PrefilledName  = user.FullName;
                PrefilledEmail = user.Email ?? "";
                PrefilledPhone = user.PhoneNumber ?? "";
            }
        }
    }

    // AJAX: POST /Booking?handler=CreatePaymentIntent
    public async Task<IActionResult> OnPostCreatePaymentIntentAsync(
        [FromForm] int carId,
        [FromForm] string startDate,
        [FromForm] string endDate)
    {
        var car = await _db.Cars.FindAsync(carId);
        if (car == null)
            return new JsonResult(new { error = "Car not found." }) { StatusCode = 400 };

        if (!DateTime.TryParse(startDate, out var start) || !DateTime.TryParse(endDate, out var end))
            return new JsonResult(new { error = "Invalid dates." }) { StatusCode = 400 };

        int days = (int)(end - start).TotalDays;
        if (days <= 0)
            return new JsonResult(new { error = "End date must be after start date." }) { StatusCode = 400 };

        long amountPence = (long)(days * car.PricePerDay * 100);

        var options = new PaymentIntentCreateOptions
        {
            Amount = amountPence,
            Currency = "gbp",
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions { Enabled = true }
        };

        var service = new PaymentIntentService();
        var intent = await service.CreateAsync(options);

        return new JsonResult(new { clientSecret = intent.ClientSecret, amountPence });
    }

    public async Task OnPostAsync()
    {
        SelectedCar = await _db.Cars.FindAsync(CarId);
        StripePublishableKey = _config["Stripe:PublishableKey"] ?? "";

        // Force email to match account for logged-in users — cannot be overridden
        if (User.Identity?.IsAuthenticated == true)
        {
            CustomerEmail = User.Identity.Name!;
            IsLoggedIn = true;
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                PrefilledName  = user.FullName;
                PrefilledEmail = user.Email ?? "";
                PrefilledPhone = user.PhoneNumber ?? "";
            }
        }

        // Verify the Stripe payment went through
        if (string.IsNullOrEmpty(PaymentIntentId))
        {
            ErrorMessage = "Payment was not completed. Please try again.";
            return;
        }

        var service = new PaymentIntentService();
        PaymentIntent intent;
        try
        {
            intent = await service.GetAsync(PaymentIntentId);
        }
        catch
        {
            ErrorMessage = "Could not verify payment. Please try again.";
            return;
        }

        if (intent.Status != "succeeded")
        {
            ErrorMessage = "Payment was not successful. Please try again.";
            return;
        }

        // Find car in BST
        var results = _bst.SearchByPriceRange(0, decimal.MaxValue);
        Car? car = null;
        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].Id == CarId)
            {
                car = results[i];
                break;
            }
        }

        if (car == null)
        {
            ErrorMessage = "Car not found. Please go back and select a car.";
            return;
        }

        if (!car.IsAvailable)
        {
            ErrorMessage = "This car is no longer available.";
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
