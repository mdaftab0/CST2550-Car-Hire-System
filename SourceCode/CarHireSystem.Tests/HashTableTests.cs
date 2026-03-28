using NUnit.Framework;
using CarHireSystem.DataStructures;
using CarHireSystem.Models;

namespace CarHireSystem.Tests;

[TestFixture]
public class HashTableTests
{
    private HashTable _hashTable = null!;
    private static readonly DateTime Start = new DateTime(2026, 4, 1);
    private static readonly DateTime End   = new DateTime(2026, 4, 5);

    [SetUp]
    public void SetUp()
    {
        _hashTable = new HashTable();
    }

    [Test]
    public void Insert_Booking_CanBeRetrievedById()
    {
        var booking = new Booking(1, 1, "Alice Smith", "alice@test.com", "07700000001", Start, End, 35.00m);
        _hashTable.Insert(booking);

        var result = _hashTable.GetById(1);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.BookingID, Is.EqualTo(1));
    }

    [Test]
    public void GetById_ReturnsCorrectBooking()
    {
        var b1 = new Booking(10, 1, "Alice Smith",  "alice@test.com", "07700000001", Start, End, 35.00m);
        var b2 = new Booking(20, 2, "Bob Jones",    "bob@test.com",   "07700000002", Start, End, 95.00m);
        _hashTable.Insert(b1);
        _hashTable.Insert(b2);

        var result = _hashTable.GetById(20);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.CustomerName, Is.EqualTo("Bob Jones"));
    }

    [Test]
    public void GetById_MissingId_ReturnsNull()
    {
        var booking = new Booking(1, 1, "Alice Smith", "alice@test.com", "07700000001", Start, End, 35.00m);
        _hashTable.Insert(booking);

        var result = _hashTable.GetById(999);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void Remove_DeletesBooking_GetByIdReturnsNull()
    {
        var booking = new Booking(5, 1, "Alice Smith", "alice@test.com", "07700000001", Start, End, 35.00m);
        _hashTable.Insert(booking);

        _hashTable.Remove(5);
        var result = _hashTable.GetById(5);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void Insert_CollisionHandling_BothBookingsRetrievable()
    {
        // IDs 1 and 17 both hash to bucket 1 (n % 16)
        var b1 = new Booking(1,  1, "Alice Smith", "alice@test.com", "07700000001", Start, End, 35.00m);
        var b2 = new Booking(17, 2, "Bob Jones",   "bob@test.com",   "07700000002", Start, End, 95.00m);
        _hashTable.Insert(b1);
        _hashTable.Insert(b2);

        var result1 = _hashTable.GetById(1);
        var result2 = _hashTable.GetById(17);

        Assert.That(result1, Is.Not.Null);
        Assert.That(result1!.CustomerName, Is.EqualTo("Alice Smith"));
        Assert.That(result2, Is.Not.Null);
        Assert.That(result2!.CustomerName, Is.EqualTo("Bob Jones"));
    }
}
