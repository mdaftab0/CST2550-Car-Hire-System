using CarHireSystem.Database;
using CarHireSystem.DataStructures;
using CarHireSystem.Models;
using CarHireSystem.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CarHireDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddRazorPages();

// Create BST and HashTable
var bst = new BinarySearchTree();
var hashTable = new HashTable();

builder.Services.AddSingleton(bst);
builder.Services.AddSingleton(hashTable);
builder.Services.AddSingleton<SearchService>();
builder.Services.AddSingleton<BookingService>();

var app = builder.Build();

// Seed cars into DB and load into BST on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CarHireDbContext>();

    if (!db.Cars.Any())
    {
        db.Cars.AddRange(
            new Car(1, "Toyota", "Corolla", "AB12CDE", 35.00m, 5),
            new Car(2, "BMW", "X5", "XY99ZZZ", 95.00m, 5),
            new Car(3, "Ford", "Fiesta", "FD21ABC", 28.00m, 5),
            new Car(4, "Mercedes", "C-Class", "MC55DEF", 120.00m, 5),
            new Car(5, "Vauxhall", "Astra", "VA33GHI", 45.00m, 5)
        );
        db.SaveChanges();
    }

    // Load all cars from DB into BST
    foreach (var car in db.Cars.ToList())
    {
        bst.Insert(car);
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();