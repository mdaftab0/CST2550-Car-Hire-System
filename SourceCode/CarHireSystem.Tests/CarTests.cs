using NUnit.Framework;
using CarHireSystem.Models;

namespace CarHireSystem.Tests;

[TestFixture]
public class CarTests
{
    [Test]
    public void Constructor_SetsAllProperties()
    {
        var car = new Car(3, "Ford", "Fiesta", "FD21ABC", 28.00m, 5);

        Assert.That(car.Id,           Is.EqualTo(3));
        Assert.That(car.Make,         Is.EqualTo("Ford"));
        Assert.That(car.Registration, Is.EqualTo("FD21ABC"));
        Assert.That(car.PricePerDay,  Is.EqualTo(28.00m));
        Assert.That(car.Seats,        Is.EqualTo(5));
    }

    [Test]
    public void IsAvailable_DefaultsToTrue()
    {
        var car = new Car(1, "Toyota", "Corolla", "AB12CDE", 35.00m, 5);

        Assert.That(car.IsAvailable, Is.True);
    }

    [Test]
    public void ToString_ContainsIdMakeRegistrationAndPrice()
    {
        var car = new Car(1, "Toyota", "Corolla", "AB12CDE", 35.00m, 5);
        var result = car.ToString();

        Assert.That(result, Does.Contain("1"));
        Assert.That(result, Does.Contain("Toyota"));
        Assert.That(result, Does.Contain("AB12CDE"));
        Assert.That(result, Does.Contain("35"));
    }

    [Test]
    public void ToString_AvailableCar_ContainsAvailableStatus()
    {
        var car = new Car(1, "Toyota", "Corolla", "AB12CDE", 35.00m, 5);

        Assert.That(car.ToString(), Does.Contain("Available"));
    }

    [Test]
    public void ToString_UnavailableCar_ContainsUnavailableStatus()
    {
        var car = new Car(1, "Toyota", "Corolla", "AB12CDE", 35.00m, 5);
        car.IsAvailable = false;

        Assert.That(car.ToString(), Does.Contain("Unavailable"));
    }
}
