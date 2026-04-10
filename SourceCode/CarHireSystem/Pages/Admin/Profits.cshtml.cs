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

        var allBookings = await _db.Bookings.ToListAsync();

        if (Period == "lifetime")
        {
            TotalEarnings = allBookings.Sum(b => b.TotalCost);
            BookingCount  = allBookings.Count;
        }
        else
        {
            var periodStart = Period switch
            {
                "daily"   => today,
                "weekly"  => today.AddDays(-6),
                "monthly" => today.AddDays(-29),
                _         => today
            };
            var periodEnd = today;

            decimal earnings = 0;
            int count = 0;

            foreach (var b in allBookings)
            {
                var bookingStart = b.StartDate.Date;
                var bookingEnd   = b.EndDate.Date;

                // Overlap between booking span and selected period
                var overlapStart = bookingStart > periodStart ? bookingStart : periodStart;
                var overlapEnd   = bookingEnd   < periodEnd   ? bookingEnd   : periodEnd;

                int overlapDays = (int)(overlapEnd - overlapStart).TotalDays + 1;
                if (overlapDays <= 0) continue;

                // Derive per-day rate from TotalCost
                int bookingDays = (int)(bookingEnd - bookingStart).TotalDays;
                if (bookingDays <= 0) bookingDays = 1;

                decimal perDayRate = b.TotalCost / bookingDays;
                earnings += perDayRate * overlapDays;
                count++;
            }

            TotalEarnings = earnings;
            BookingCount  = count;
        }

        AverageEarning = BookingCount > 0 ? TotalEarnings / BookingCount : 0;

        return Page();
    }
}
