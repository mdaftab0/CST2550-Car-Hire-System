using Microsoft.AspNetCore.Identity;

namespace CarHireSystem.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = "";
}
