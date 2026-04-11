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

    public int     TotalCars        { get; set; }
    public int     AvailableCars    { get; set; }
    public int     TotalBookings    { get; set; }
    public int     ActiveBookings   { get; set; }
    public int     Utilisation      { get; set; }  // percentage of fleet currently on hire
    public decimal RevenueThisMonth { get; set; }

    public async Task OnGetAsync()
    {
        TotalCars      = await _db.Cars.CountAsync();
        AvailableCars  = await _db.Cars.CountAsync(c => c.IsAvailable);
        TotalBookings  = await _db.Bookings.CountAsync();
        ActiveBookings = await _db.Bookings.CountAsync(b => b.IsActive);

        Utilisation = TotalCars > 0
            ? (int)Math.Round((double)(TotalCars - AvailableCars) / TotalCars * 100)
            : 0;

        RevenueThisMonth = await CalcRevenueThisMonthAsync();
    }

    private async Task<decimal> CalcRevenueThisMonthAsync()
    {
        var today       = DateTime.UtcNow.Date;
        var periodStart = today.AddDays(-29);

        var bookings = await _db.Bookings
            .Where(b => !b.IsCancelled)
            .ToListAsync();

        decimal total = 0;
        foreach (var b in bookings)
        {
            var start = b.StartDate.Date;
            var end   = b.EndDate.Date;

            var overlapStart = start > periodStart ? start : periodStart;
            var overlapEnd   = end   < today       ? end   : today;

            int overlapDays = (int)(overlapEnd - overlapStart).TotalDays + 1;
            if (overlapDays <= 0) continue;

            int bookingDays = (int)(end - start).TotalDays;
            if (bookingDays <= 0) bookingDays = 1;

            total += b.TotalCost / bookingDays * overlapDays;
        }

        return total;
    }
}
