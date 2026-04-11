using CarHireSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarHireSystem.Pages.Account;

public class ResetPasswordModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ResetPasswordModel(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [BindProperty(SupportsGet = true)] public string Email { get; set; } = "";
    [BindProperty(SupportsGet = true)] public string Token { get; set; } = "";

    [BindProperty] public string NewPassword    { get; set; } = "";
    [BindProperty] public string ConfirmPassword { get; set; } = "";

    public string?  ErrorMessage   { get; set; }
    public bool     ResetSucceeded { get; set; }

    public IActionResult OnGet()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Token))
            return RedirectToPage("/Account/ForgotPassword");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (NewPassword != ConfirmPassword)
        {
            ErrorMessage = "Passwords do not match.";
            return Page();
        }

        if (NewPassword.Length < 8 ||
            !NewPassword.Any(char.IsUpper) ||
            !NewPassword.Any(char.IsLower) ||
            !NewPassword.Any(char.IsDigit))
        {
            ErrorMessage = "Password must be at least 8 characters and include an uppercase letter, a lowercase letter, and a digit.";
            return Page();
        }

        var user = await _userManager.FindByEmailAsync(Email);
        if (user == null)
        {
            // Don't reveal account existence — show success anyway
            ResetSucceeded = true;
            return Page();
        }

        var result = await _userManager.ResetPasswordAsync(user, Token, NewPassword);
        if (result.Succeeded)
        {
            ResetSucceeded = true;
            return Page();
        }

        ErrorMessage = string.Join(" ", result.Errors.Select(e => e.Description));
        return Page();
    }
}
