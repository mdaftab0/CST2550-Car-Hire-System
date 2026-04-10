using NUnit.Framework;
using CarHireSystem.Models;

namespace CarHireSystem.Tests;

[TestFixture]
public class BookingTests
{
    [Test]
    public void Constructor_SetsAllFieldsCorrectly()
    {
        var start = new DateTime(2026, 4, 1);
        var end   = new DateTime(2026, 4, 5);

        var booking = new Booking(42, 3, "Jane Doe", "jane@test.com", "07700123456", start, end, 35.00m);

        Assert.That(booking.BookingID,     Is.EqualTo(42));
        Assert.That(booking.CarID,         Is.EqualTo(3));
        Assert.That(booking.CustomerName,  Is.EqualTo("Jane Doe"));
        Assert.That(booking.CustomerEmail, Is.EqualTo("jane@test.com"));
        Assert.That(booking.CustomerPhone, Is.EqualTo("07700123456"));
        Assert.That(booking.StartDate,     Is.EqualTo(start));
        Assert.That(booking.EndDate,       Is.EqualTo(end));
    }

    [Test]
    public void IsActive_DefaultsToTrue()
    {
        var booking = new Booking(1, 1, "Jane Doe", "jane@test.com", "07700123456",
            new DateTime(2026, 4, 1), new DateTime(2026, 4, 3), 50.00m);

        Assert.That(booking.IsActive, Is.True);
    }

    [Test]
    public void TotalCost_IsCalculatedAsDaysTimesPrice()
    {
        var start = new DateTime(2026, 4, 1);
        var end   = new DateTime(2026, 4, 6); // 5 days

        var booking = new Booking(1, 1, "Jane Doe", "jane@test.com", "07700123456", start, end, 40.00m);

        Assert.That(booking.TotalCost, Is.EqualTo(200.00m)); // 5 × £40
    }

    [Test]
    public void TotalCost_OneDayHire_EqualsPricePerDay()
    {
        var start = new DateTime(2026, 4, 1);
        var end   = new DateTime(2026, 4, 2); // 1 day

        var booking = new Booking(1, 1, "Jane Doe", "jane@test.com", "07700123456", start, end, 95.00m);

        Assert.That(booking.TotalCost, Is.EqualTo(95.00m));
    }

    [Test]
    public void TotalCost_ZeroDays_ChargesMinimumOneDay()
    {
        var date = new DateTime(2026, 4, 1);

        var booking = new Booking(1, 1, "Jane Doe", "jane@test.com", "07700123456", date, date, 35.00m);

        Assert.That(booking.TotalCost, Is.EqualTo(35.00m));
    }
}
