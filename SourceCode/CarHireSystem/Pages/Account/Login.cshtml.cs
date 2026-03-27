using CarHireSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarHireSystem.Pages.Account;

public class LoginModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public LoginModel(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    [BindProperty] public string Email { get; set; } = "";
    [BindProperty] public string Password { get; set; } = "";
    [BindProperty] public bool RememberMe { get; set; }

    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        var result = await _signInManager.PasswordSignInAsync(Email, Password, RememberMe, lockoutOnFailure: false);

        if (result.Succeeded)
            return RedirectToPage("/Index");

        ErrorMessage = "Invalid email or password.";
        return Page();
    }
}
