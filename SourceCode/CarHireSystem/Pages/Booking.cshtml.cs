using CarHireSystem.DataStructures;
using CarHireSystem.Models;
using CarHireSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarHireSystem.Pages;

public class BookingModel : PageModel
{
    private readonly BookingService _bookingService;
    private readonly BinarySearchTree _bst;

    public BookingModel(BookingService bookingService, BinarySearchTree bst)
    {
        _bookingService = bookingService;
        _bst = bst;
    }

    [BindProperty] public int CarId { get; set; }
    [BindProperty] public string CustomerName { get; set; } = "";
    [BindProperty] public string CustomerEmail { get; set; } = "";
    [BindProperty] public string CustomerPhone { get; set; } = "";
    [BindProperty] public DateTime StartDate { get; set; }
    [BindProperty] public DateTime EndDate { get; set; }

    public Booking? Confirmation { get; set; }
    public string? ErrorMessage { get; set; }

    public void OnPost()
    {
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
            ErrorMessage = "Car not found.";
            return;
        }

        if (!car.IsAvailable)
        {
            ErrorMessage = "Car is not available.";
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
        Confirmation = booking;
    }
}