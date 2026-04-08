using System.Security.Claims;
using CarHireSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarHireSystem.Pages.Account;

public class ExternalLoginCallbackModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public ExternalLoginCallbackModel(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public string? DebugMessage { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            DebugMessage = $"GetExternalLoginInfoAsync returned null. " +
                           $"Scheme={Request.Scheme}, Host={Request.Host}, " +
                           $"QueryString={Request.QueryString}";
            return Page();
        }

        // Try to sign in with an existing linked account
        var signInResult = await _signInManager.ExternalLoginSignInAsync(
            info.LoginProvider, info.ProviderKey, isPersistent: false);

        if (signInResult.Succeeded)
            return RedirectToPage("/Index");

        var email    = info.Principal.FindFirstValue(ClaimTypes.Email) ?? "";
        var fullName = info.Principal.FindFirstValue(ClaimTypes.Name)  ?? "";

        // Check if an account with this email already exists (registered via password)
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            // Link the Google login to the existing account and sign in
            await _userManager.AddLoginAsync(existingUser, info);
            await _signInManager.SignInAsync(existingUser, isPersistent: false);
            return RedirectToPage("/Index");
        }

        // No existing account — create one from the Google profile
        var user = new ApplicationUser
        {
            UserName       = email,
            Email          = email,
            FullName       = fullName,
            EmailConfirmed = true
        };

        var createResult = await _userManager.CreateAsync(user);
        if (!createResult.Succeeded)
        {
            DebugMessage = "Account creation failed: " +
                           string.Join(", ", Array.ConvertAll(createResult.Errors.ToArray(), e => e.Description));
            return Page();
        }

        await _userManager.AddLoginAsync(user, info);
        await _userManager.AddToRoleAsync(user, "Customer");
        await _signInManager.SignInAsync(user, isPersistent: false);

        return RedirectToPage("/Index");
    }
}
