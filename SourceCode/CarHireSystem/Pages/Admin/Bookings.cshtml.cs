using CarHireSystem.Database;
using CarHireSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CarHireSystem.Pages.Admin;

[Authorize(Roles = "Admin")]
public class BookingsModel : PageModel
{
    private readonly CarHireDbContext _db;

    public BookingsModel(CarHireDbContext db)
    {
        _db = db;
    }

    public List<Booking> Bookings { get; set; } = new();
    public Dictionary<int, Car> CarsById { get; set; } = new();

    public async Task OnGetAsync()
    {
        Bookings = await _db.Bookings.OrderByDescending(b => b.BookingID).ToListAsync();

        CarsById = await _db.Cars
            .ToDictionaryAsync(c => c.Id);
    }
}
