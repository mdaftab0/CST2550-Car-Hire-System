using CarHireSystem.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CarHireSystem.Pages.Admin;

[Authorize(Roles = "Admin")]
public class ProfitsModel : PageModel
{
    private readonly CarHireDbContext _db;

    public ProfitsModel(CarHireDbContext db)
    {
        _db = db;
    }

    public string Period { get; set; } = "lifetime";
    public decimal TotalEarnings { get; set; }
    public int BookingCount { get; set; }
    public decimal AverageEarning { get; set; }

    public async Task<IActionResult> OnGetAsync(string period = "lifetime")
    {
        Period = period.ToLower() switch
        {
            "daily" or "weekly" or "monthly" or "lifetime" => period.ToLower(),
            _ => "lifetime"
        };

        var today = DateTime.UtcNow.Date;

        DateTime? cutoff = Period switch
        {
            "daily"   => today,
            "weekly"  => today.AddDays(-6),
            "monthly" => today.AddDays(-29),
            _         => null
        };

        var query = _db.Bookings.AsQueryable();
        if (cutoff.HasValue)
            query = query.Where(b => b.StartDate.Date >= cutoff.Value);

        var costs = await query.Select(b => b.TotalCost).ToListAsync();

        BookingCount   = costs.Count;
        TotalEarnings  = costs.Sum();
        AverageEarning = BookingCount > 0 ? TotalEarnings / BookingCount : 0;

        return Page();
    }
}
