using CarHireSystem.Database;
using CarHireSystem.DataStructures;
using CarHireSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CarHireSystem.Pages.Admin;

[Authorize(Roles = "Admin")]
public class CarsModel : PageModel
{
    private readonly CarHireDbContext _db;
    private readonly BinarySearchTree _bst;

    public CarsModel(CarHireDbContext db, BinarySearchTree bst)
    {
        _db = db;
        _bst = bst;
    }

    // ── Page state ────────────────────────────────────────────
    public Car[] Cars { get; set; } = Array.Empty<Car>();
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    // ── Add car fields ────────────────────────────────────────
    [BindProperty] public string NewMake { get; set; } = "";
    [BindProperty] public string NewModel { get; set; } = "";
    [BindProperty] public string NewRegistration { get; set; } = "";
    [BindProperty] public decimal NewPricePerDay { get; set; }
    [BindProperty] public int NewSeats { get; set; }
    [BindProperty] public string? NewPhotoUrl { get; set; }

    // ── Edit car fields ───────────────────────────────────────
    [BindProperty] public int EditId { get; set; }
    [BindProperty] public string EditMake { get; set; } = "";
    [BindProperty] public string EditModel { get; set; } = "";
    [BindProperty] public string EditRegistration { get; set; } = "";
    [BindProperty] public decimal EditPricePerDay { get; set; }
    [BindProperty] public int EditSeats { get; set; }
    [BindProperty] public bool EditIsAvailable { get; set; }
    [BindProperty] public string? EditPhotoUrl { get; set; }

    // ID of the car currently being edited (from query string)
    public int? ActiveEditId { get; set; }

    // ── Handlers ──────────────────────────────────────────────
    public async Task OnGetAsync(int? editId)
    {
        ActiveEditId = editId;
        Cars = await _db.Cars.OrderBy(c => c.Id).ToArrayAsync();
    }

    public async Task<IActionResult> OnPostAddAsync()
    {
        if (string.IsNullOrWhiteSpace(NewMake) || string.IsNullOrWhiteSpace(NewModel)
            || string.IsNullOrWhiteSpace(NewRegistration) || NewPricePerDay <= 0 || NewSeats <= 0)
        {
            ErrorMessage = "All fields are required and price/seats must be greater than zero.";
            Cars = await _db.Cars.OrderBy(c => c.Id).ToArrayAsync();
            return Page();
        }

        int nextId = _db.Cars.Any() ? _db.Cars.Max(c => c.Id) + 1 : 1;

        var car = new Car(nextId, NewMake, NewModel, NewRegistration, NewPricePerDay, NewSeats);
        car.PhotoUrl = NewPhotoUrl;

        _db.Cars.Add(car);
        await _db.SaveChangesAsync();
        _bst.Insert(car);

        TempData["Success"] = $"{NewMake} {NewModel} added successfully.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostEditAsync()
    {
        var car = await _db.Cars.FindAsync(EditId);
        if (car == null)
        {
            TempData["Error"] = "Car not found.";
            return RedirectToPage();
        }

        car.Make = EditMake;
        car.Model = EditModel;
        car.Registration = EditRegistration;
        car.PricePerDay = EditPricePerDay;
        car.Seats = EditSeats;
        car.IsAvailable = EditIsAvailable;
        car.PhotoUrl = EditPhotoUrl;

        await _db.SaveChangesAsync();

        // Rebuild BST to reflect updated price ordering
        _bst.Clear();
        foreach (var c in await _db.Cars.ToArrayAsync())
            _bst.Insert(c);

        TempData["Success"] = $"{car.Make} {car.Model} updated successfully.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostToggleAsync(int carId)
    {
        var car = await _db.Cars.FindAsync(carId);
        if (car == null)
        {
            TempData["Error"] = "Car not found.";
            return RedirectToPage();
        }

        // Block if there is an active booking on this car
        bool hasActiveBooking = await _db.Bookings
            .AnyAsync(b => b.CarID == carId && b.IsActive && !b.IsCancelled);

        if (hasActiveBooking)
        {
            TempData["Error"] = $"{car.Make} {car.Model} has an active booking and cannot be made unavailable.";
            return RedirectToPage();
        }

        car.IsAvailable = !car.IsAvailable;
        await _db.SaveChangesAsync();

        // Sync BST
        _bst.Clear();
        foreach (var c in await _db.Cars.ToArrayAsync())
            _bst.Insert(c);

        TempData["Success"] = car.IsAvailable
            ? $"{car.Make} {car.Model} is now available."
            : $"{car.Make} {car.Model} marked as unavailable (maintenance).";

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int carId)
    {
        var car = await _db.Cars.FindAsync(carId);
        if (car == null)
        {
            TempData["Error"] = "Car not found.";
            return RedirectToPage();
        }

        _db.Cars.Remove(car);
        await _db.SaveChangesAsync();

        // Rebuild BST without the deleted car
        _bst.Clear();
        foreach (var c in await _db.Cars.ToArrayAsync())
            _bst.Insert(c);

        TempData["Success"] = $"{car.Make} {car.Model} deleted.";
        return RedirectToPage();
    }
}
