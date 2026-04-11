using CarHireSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarHireSystem.Pages.Account;

public class ForgotPasswordModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ForgotPasswordModel(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [BindProperty] public string Email { get; set; } = "";

    public string? ResetLink { get; set; }
    public bool    Submitted { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        Submitted = true;

        var user = await _userManager.FindByEmailAsync(Email);
        if (user == null)
            return Page(); // Don't reveal whether the account exists

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        ResetLink = Url.Page(
            "/Account/ResetPassword",
            pageHandler: null,
            values: new { email = Email, token },
            protocol: Request.Scheme);

        return Page();
    }
}
