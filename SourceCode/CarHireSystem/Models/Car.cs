namespace CarHireSystem.Models;

public class Car
{
    public int Id { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Registration { get; set; } = string.Empty;
    public decimal PricePerDay { get; set; }
    public int Seats { get; set; }
    public bool IsAvailable { get; set; } = true;
    public string? PhotoUrl { get; set; }

    public Car() { }

    public Car(int id, string make, string model, string registration, decimal pricePerDay, int seats)
    {
        Id = id;
        Make = make;
        Model = model;
        Registration = registration;
        PricePerDay = pricePerDay > 0 ? pricePerDay : throw new ArgumentException("Price must be positive");
        Seats = seats > 0 ? seats : throw new ArgumentException("Seats must be at least 1");
    }

    public override string ToString() => 
        $"{Id} | {Make} {Model} ({Registration}) | ${PricePerDay:F2}/day | {Seats} Seats | {(IsAvailable ? "Available" : "Unavailable")}";
}
