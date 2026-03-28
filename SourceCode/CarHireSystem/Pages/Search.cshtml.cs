using CarHireSystem.Database;
using CarHireSystem.DataStructures;
using CarHireSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CarHireSystem.Pages;

public class SearchModel : PageModel
{
    private readonly SearchService _searchService;
    private readonly CarHireDbContext _db;

    public SearchModel(SearchService searchService, CarHireDbContext db)
    {
        _searchService = searchService;
        _db = db;
    }

    [BindProperty] public decimal MinPrice { get; set; }
    [BindProperty] public decimal MaxPrice { get; set; }

    public CarArray? Results { get; set; }
    public bool IsFiltered { get; set; }

    // On page load — show every car, available ones first
    public async Task OnGetAsync()
    {
        var cars = await _db.Cars
            .OrderByDescending(c => c.IsAvailable)
            .ThenBy(c => c.PricePerDay)
            .ToListAsync();

        Results = new CarArray();
        foreach (var car in cars)
            Results.Add(car);
    }

    // On search — filter by price range via BST, sync availability from DB
    public async Task OnPostAsync()
    {
        IsFiltered = true;
        Results = _searchService.SearchByPriceRange(MinPrice, MaxPrice);

        if (Results.Count > 0)
        {
            var ids = new List<int>();
            for (int i = 0; i < Results.Count; i++)
                ids.Add(Results.Get(i).Id);

            var dbAvailability = await _db.Cars
                .Where(c => ids.Contains(c.Id))
                .ToDictionaryAsync(c => c.Id, c => c.IsAvailable);

            for (int i = 0; i < Results.Count; i++)
            {
                var car = Results.Get(i);
                if (dbAvailability.TryGetValue(car.Id, out var isAvailable))
                    car.IsAvailable = isAvailable;
            }
        }
    }
}
