using CarHireSystem.DataStructures;
using CarHireSystem.Models;
using CarHireSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarHireSystem.Pages;

public class ReturnsModel : PageModel
{
    private readonly BookingService _bookingService;
    private readonly BinarySearchTree _bst;

    public ReturnsModel(BookingService bookingService, BinarySearchTree bst)
    {
        _bookingService = bookingService;
        _bst = bst;
    }

    [BindProperty] public int BookingId { get; set; }

    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }

    public void OnPost()
    {
        Booking? booking = _bookingService.GetBooking(BookingId);

        if (booking == null)
        {
            ErrorMessage = "Booking not found.";
            return;
        }

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

        _bookingService.ReturnCar(BookingId, car);
        SuccessMessage = $"Car returned successfully. {car.Make} {car.Model} is now available.";
    }
}