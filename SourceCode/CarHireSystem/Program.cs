using CarHireSystem.DataStructures;
using CarHireSystem.Models;
using CarHireSystem.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorPages();

// Create data structures
var bst = new BinarySearchTree();
var hashTable = new HashTable();

// Seed test cars
bst.Insert(new Car(1, "Toyota", "Corolla", "AB12CDE", 35.00m, 5));
bst.Insert(new Car(2, "BMW", "X5", "XY99ZZZ", 95.00m, 5));
bst.Insert(new Car(3, "Ford", "Fiesta", "FD21ABC", 28.00m, 5));
bst.Insert(new Car(4, "Mercedes", "C-Class", "MC55DEF", 120.00m, 5));
bst.Insert(new Car(5, "Vauxhall", "Astra", "VA33GHI", 45.00m, 5));

// Register services as singletons so they share the same BST and HashTable
builder.Services.AddSingleton(bst);
builder.Services.AddSingleton(hashTable);
builder.Services.AddSingleton<SearchService>();
builder.Services.AddSingleton<BookingService>();

var app = builder.Build();

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