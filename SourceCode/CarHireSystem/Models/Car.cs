namespace CarHireSystem.Models;

public class Car
{
    public int Id { get; set; }
    public string? Make { get; set; }
    public string? Model { get; set; }
    public string? Registration { get; set; }
    public decimal PricePerDay { get; set; }
    public int Seats { get; set; }
    public bool IsAvailable { get; set; }

    public Car (int id, string make, string model, string registration, decimal pricePerDay, int seats)
    {
        Id = id;
        Make = make;
        Registration = registration;
        PricePerDay = pricePerDay;
        Seats = seats;
        IsAvailable = true;
    }

    public override string ToString()
    {
        string status = IsAvailable ? "Available" : "Unavailable";
        return $"{Id} | {Make} {Model} |  {Registration} | ${PricePerDay}/day | {Seats} Seats | {status}";
    }
}
