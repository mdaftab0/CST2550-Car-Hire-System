using CarHireSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarHireSystem.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public RegisterModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [BindProperty] public string FullName { get; set; } = "";
    [BindProperty] public string Email { get; set; } = "";
    [BindProperty] public string PhoneNumber { get; set; } = "";
    [BindProperty] public string Password { get; set; } = "";
    [BindProperty] public string ConfirmPassword { get; set; } = "";

    public List<string> ErrorMessages { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        if (Password != ConfirmPassword)
        {
            ErrorMessages.Add("Passwords do not match.");
            return Page();
        }

        var user = new ApplicationUser
        {
            UserName = Email,
            Email = Email,
            FullName = FullName,
            PhoneNumber = PhoneNumber,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, Password);
        if (!result.Succeeded)
        {
            ErrorMessages.AddRange(result.Errors.Select(e => e.Description));
            return Page();
        }

        await _userManager.AddToRoleAsync(user, "Customer");
        await _signInManager.SignInAsync(user, isPersistent: false);
        return RedirectToPage("/Index");
    }
}
