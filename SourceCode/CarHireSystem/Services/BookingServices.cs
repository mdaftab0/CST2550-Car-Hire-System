using CarHireSystem.Models;
using CarHireSystem.DataStructures;
namespace CarHireSystem.Services;

public class BookingService
{
    private HashTable _hashTable;

    public BookingService(HashTable hashTable)
    {
        _hashTable = hashTable;
    }

    public void CreateBooking(Booking booking)
    {
        _hashTable.Insert(booking);
    }

    public Booking? GetBooking(int bookingId)
    {
        return _hashTable.GetById(bookingId);
    }

    public void ReturnCar(int bookingId, Car car)
    {
        Booking? booking = _hashTable.GetById(bookingId);
        if (booking != null)
        {
            booking.Booked = false;
            car.IsAvailable = true;
        }
    }
}