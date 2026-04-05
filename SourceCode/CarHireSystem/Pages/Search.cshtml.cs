using CarHireSystem.Database;
using CarHireSystem.DataStructures;
using CarHireSystem.Services;
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

    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }

    public CarArray? Results { get; set; }
    public bool IsFiltered { get; set; }

    public async Task OnGetAsync()
    {
        var cars = await _db.Cars
            .OrderByDescending(c => c.IsAvailable)
            .ThenBy(c => c.PricePerDay)
            .ToArrayAsync();

        Results = new CarArray();
        foreach (var car in cars)
            Results.Add(car);
    }

    public async Task OnPostAsync(decimal? minPrice, decimal? maxPrice)
    {
        MinPrice = minPrice;
        MaxPrice = maxPrice;
        IsFiltered = true;

        Results = _searchService.SearchByPriceRange(minPrice ?? 0, maxPrice ?? decimal.MaxValue);

        if (Results.Count > 0)
        {
            var ids = Enumerable.Range(0, Results.Count).Select(i => Results.Get(i).Id).ToArray();

            var dbCars = await _db.Cars
                .Where(c => ids.Contains(c.Id))
                .ToArrayAsync();

            for (int i = 0; i < Results.Count; i++)
            {
                var car = Results.Get(i);
                var dbCar = Array.Find(dbCars, c => c.Id == car.Id);
                if (dbCar != null)
                {
                    car.IsAvailable = dbCar.IsAvailable;
                    car.PhotoUrl = dbCar.PhotoUrl;
                }
            }
        }
    }
}
