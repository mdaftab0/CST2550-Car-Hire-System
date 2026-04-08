using CarHireSystem.Database;
using CarHireSystem.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CarHireSystem.Pages;

public class IndexModel : PageModel
{
    private readonly CarHireDbContext _db;

    public IndexModel(CarHireDbContext db)
    {
        _db = db;
    }

    public Car[] AvailableCars { get; set; } = Array.Empty<Car>();

    public async Task OnGetAsync()
    {
        AvailableCars = await _db.Cars
            .Where(c => c.IsAvailable)
            .OrderBy(c => c.PricePerDay)
            .ToArrayAsync();
    }
}
