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
}