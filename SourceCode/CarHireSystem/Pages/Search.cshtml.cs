using CarHireSystem.DataStructures;
using CarHireSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarHireSystem.Pages;

public class SearchModel : PageModel
{
    private readonly SearchService _searchService;

    public SearchModel(SearchService searchService)
    {
        _searchService = searchService;
    }

    [BindProperty]
    public decimal MinPrice { get; set; }

    [BindProperty]
    public decimal MaxPrice { get; set; }

    public CarArray? Results { get; set; }

    public void OnPost()
    {
        Results = _searchService.SearchByPriceRange(MinPrice, MaxPrice);
    }
}