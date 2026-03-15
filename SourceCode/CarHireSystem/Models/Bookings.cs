namespace CarHireSystem.Models;

public class Booking
{
    public int BookingID { get; set; }
    public int CarID { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool Booked { get; set; }
    public decimal TotalCost { get; set; }

    public Booking(int bookingID, int carID, string customerName, string customerEmail, string customerPhone, DateTime startDate,
        DateTime endDate, decimal pricePerDay)
    {
        BookingID = bookingID;
        CarID = carID;
        CustomerName = customerName;
        CustomerEmail = customerEmail;
        CustomerPhone = customerPhone;
        StartDate = startDate;
        EndDate = endDate;
        Booked = true;
        
        TotalCost = (endDate - startDate).Days * pricePerDay;
    }

    public override string ToString()
    {
        string status = Booked ? "Booked" : "Not Booked";
        return $"{BookingID} - {status} | {CarID} | {CustomerName} | {CustomerEmail} {CustomerPhone}| {StartDate} - {EndDate}";
    }
}