using CarHireSystem.Database;
using CarHireSystem.DataStructures;
using CarHireSystem.Models;
using CarHireSystem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<CarHireDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<CarHireDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId     = builder.Configuration["Authentication:Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
    });

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/Login";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
});

builder.Services.AddRazorPages();
builder.Services.AddHttpClient();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Custom data structure services
var bst = new BinarySearchTree();
var hashTable = new HashTable();

builder.Services.AddSingleton(bst);
builder.Services.AddSingleton(hashTable);
builder.Services.AddSingleton<SearchService>();
builder.Services.AddSingleton<BookingService>();

var app = builder.Build();

// Seed data and Identity roles/admin on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CarHireDbContext>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // Seed cars
    if (!db.Cars.Any())
    {
        db.Cars.AddRange(
            new Car(1, "Toyota", "Corolla", "AB12CDE", 35.00m, 5) { PhotoUrl = "https://placehold.co/600x260/0f172a/ffffff?text=Toyota+Corolla" },
            new Car(2, "BMW", "X5", "XY99ZZZ", 95.00m, 5) { PhotoUrl = "https://placehold.co/600x260/0f172a/ffffff?text=BMW+X5" },
            new Car(3, "Ford", "Fiesta", "FD21ABC", 28.00m, 5) { PhotoUrl = "https://placehold.co/600x260/0f172a/ffffff?text=Ford+Fiesta" },
            new Car(4, "Mercedes", "C-Class", "MC55DEF", 120.00m, 5) { PhotoUrl = "https://placehold.co/600x260/0f172a/ffffff?text=Mercedes+C-Class" },
            new Car(5, "Vauxhall", "Astra", "VA33GHI", 45.00m, 5) { PhotoUrl = "https://placehold.co/600x260/0f172a/ffffff?text=Vauxhall+Astra" },
            new Car(6, "Porsche", "Taycan", "PO45CH3", 75.00m, 5) { PhotoUrl = "https://placehold.co/600x260/0f172a/ffffff?text=Porsche+Taycan" },
            new Car(7, "Ford", "Raptor", "RA970R", 45.00m, 5) { PhotoUrl = "https://placehold.co/600x260/0f172a/ffffff?text=Ford+Raptor" },
            new Car(8, "Land Rover", "Defender", "M0N37", 150.00m, 7) { PhotoUrl = "https://placehold.co/600x260/0f172a/ffffff?text=Land+Rover+Defender" },
            new Car(9, "Volkswagen", "Golf", "B4NG37", 20.00m, 5) { PhotoUrl = "https://placehold.co/600x260/0f172a/ffffff?text=Volkswagen+Golf" },
            new Car(10, "MiniCooper", "Countryman", "QW09OP", 30.00m, 5) { PhotoUrl = "https://placehold.co/600x260/0f172a/ffffff?text=MiniCooper+Countryman" }
        );
        db.SaveChanges();
    }

    // Load all cars into BST
    foreach (var car in db.Cars.ToList())
        bst.Insert(car);

    // Seed roles
    foreach (var role in new[] { "Admin", "Customer" })
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    // Seed default admin account
    const string adminEmail = "admin@easyhire.com";
    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var admin = new ApplicationUser
        {
            UserName    = adminEmail,
            Email       = adminEmail,
            FullName    = "EasyHire Admin",
            EmailConfirmed = true
        };
        var result = await userManager.CreateAsync(admin, "Admin123!");
        if (result.Succeeded)
            await userManager.AddToRoleAsync(admin, "Admin");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

app.Run();
