using CarHireSystem.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CarHireSystem.Pages.Admin;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly CarHireDbContext _db;

    public IndexModel(CarHireDbContext db)
    {
        _db = db;
    }

    public int TotalCars { get; set; }
    public int AvailableCars { get; set; }
    public int TotalBookings { get; set; }
    public int ActiveBookings { get; set; }

    public async Task OnGetAsync()
    {
        TotalCars     = await _db.Cars.CountAsync();
        AvailableCars = await _db.Cars.CountAsync(c => c.IsAvailable);
        TotalBookings = await _db.Bookings.CountAsync();
        ActiveBookings = await _db.Bookings.CountAsync(b => b.Booked);
    }
}
