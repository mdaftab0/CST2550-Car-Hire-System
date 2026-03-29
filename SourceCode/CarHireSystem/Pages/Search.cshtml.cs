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

    [BindProperty] public decimal? MinPrice { get; set; }
    [BindProperty] public decimal? MaxPrice { get; set; }
    [BindProperty] public string? SearchTerm { get; set; }

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

    public async Task OnPostAsync()
    {
        IsFiltered = true;

        bool hasTerm  = !string.IsNullOrWhiteSpace(SearchTerm);
        bool hasPrice = MinPrice.HasValue || MaxPrice.HasValue;

        if (hasPrice)
        {
            // Use BST for price range, then apply text filter in memory
            decimal min = MinPrice ?? 0;
            decimal max = MaxPrice ?? decimal.MaxValue;

            var bstResults = _searchService.SearchByPriceRange(min, max);

            // Sync availability from DB
            if (bstResults.Count > 0)
            {
                var ids = Enumerable.Range(0, bstResults.Count).Select(i => bstResults.Get(i).Id).ToList();
                var dbCars = await _db.Cars
                    .Where(c => ids.Contains(c.Id))
                    .ToDictionaryAsync(c => c.Id, c => c);

                for (int i = 0; i < bstResults.Count; i++)
                {
                    var car = bstResults.Get(i);
                    if (dbCars.TryGetValue(car.Id, out var dbCar))
                    {
                        car.IsAvailable = dbCar.IsAvailable;
                        car.PhotoUrl = dbCar.PhotoUrl;
                    }
                }
            }

            if (hasTerm)
            {
                // Filter BST results by search term
                var term = SearchTerm!.Trim().ToLower();
                var filtered = new CarArray();
                for (int i = 0; i < bstResults.Count; i++)
                {
                    var car = bstResults.Get(i);
                    if ((car.Make?.ToLower().Contains(term) ?? false) ||
                        (car.Model?.ToLower().Contains(term) ?? false) ||
                        (car.Registration?.ToLower().Contains(term) ?? false))
                    {
                        filtered.Add(car);
                    }
                }
                Results = filtered;
            }
            else
            {
                Results = bstResults;
            }
        }
        else if (hasTerm)
        {
            // Text-only search — query DB directly
            var term = SearchTerm!.Trim().ToLower();
            var cars = await _db.Cars
                .Where(c => c.Make!.ToLower().Contains(term) ||
                            c.Model!.ToLower().Contains(term) ||
                            c.Registration!.ToLower().Contains(term))
                .OrderByDescending(c => c.IsAvailable)
                .ThenBy(c => c.PricePerDay)
                .ToListAsync();

            Results = new CarArray();
            foreach (var car in cars)
                Results.Add(car);
        }
        else
        {
            // No filters — show all
            IsFiltered = false;
            await OnGetAsync();
        }
    }
}
