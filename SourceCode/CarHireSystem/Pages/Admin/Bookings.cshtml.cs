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

    public Booking[] Bookings { get; set; } = Array.Empty<Booking>();
    public Car[] Cars { get; set; } = Array.Empty<Car>();

    public async Task OnGetAsync()
    {
        Bookings = await _db.Bookings.OrderByDescending(b => b.BookingID).ToArrayAsync();
        Cars = await _db.Cars.ToArrayAsync();
    }
}
