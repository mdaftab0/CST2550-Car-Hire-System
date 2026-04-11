using System.ComponentModel.DataAnnotations;
namespace CarHireSystem.Models;

public class Booking
{
    public int BookingID { get; set; }
    public int CarID { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public decimal TotalCost { get; private set; }

    public Booking() { }

    public Booking(int bookingID, int carID, string name, string email, string phone, DateTime start, DateTime end, decimal pricePerDay)
    {
        if (end < start) throw new ArgumentException("End date cannot be before start date.");

        BookingID = bookingID;
        CarID = carID;
        CustomerName = name;
        CustomerEmail = email;
        CustomerPhone = phone;
        StartDate = start;
        EndDate = end;
        IsActive = true;

        // Calculate duration (minimum 1 day charge)
        int days = (end - start).Days;
        if (days <= 0) days = 1; 
        TotalCost = days * pricePerDay;
    }

    public void UpdateTerms(DateTime start, DateTime end, string name, string phone, decimal pricePerDay)
    {
        StartDate    = start;
        EndDate      = end;
        CustomerName = name;
        CustomerPhone = phone;
        int days = (end - start).Days;
        if (days <= 0) days = 1;
        TotalCost = days * pricePerDay;
    }

    public override string ToString() =>
        $"Booking #{BookingID} [Car: {CarID}] - {CustomerName} | {StartDate:yyyy-MM-dd} to {EndDate:yyyy-MM-dd} | Total: ${TotalCost:F2}";
}
