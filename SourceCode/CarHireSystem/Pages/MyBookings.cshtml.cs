using CarHireSystem.Database;
using CarHireSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CarHireSystem.Pages;

[Authorize]
public class MyBookingsModel : PageModel
{
    private readonly CarHireDbContext _db;

    public MyBookingsModel(CarHireDbContext db)
    {
        _db = db;
    }

    public List<Booking> Bookings { get; set; } = new();
    public Dictionary<int, Car> CarsById { get; set; } = new();

    public async Task OnGetAsync()
    {
        var email = User.Identity!.Name!;

        Bookings = await _db.Bookings
            .Where(b => b.CustomerEmail == email)
            .OrderByDescending(b => b.BookingID)
            .ToListAsync();

        var carIds = Bookings.Select(b => b.CarID).Distinct().ToList();
        CarsById = await _db.Cars
            .Where(c => carIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c);
    }
}
