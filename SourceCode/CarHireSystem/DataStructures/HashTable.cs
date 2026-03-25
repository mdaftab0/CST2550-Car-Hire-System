using CarHireSystem.Models;
namespace CarHireSystem.DataStructures;

public class HashTable
{
    private class Node
    {
        public Booking Data;
        public Node? Next;

        public Node(Booking booking)
        {
            Data = booking;
            Next = null;
        }
    }

    private Node?[] _buckets = new Node?[16];

    private int GetBucket(int id)
    {
        return id % 16;
    }

    public void Insert(Booking booking)
    {
        int index = GetBucket(booking.BookingID);
        Node newNode = new Node(booking);
        newNode.Next = _buckets[index];
        _buckets[index] = newNode;
    }

    public Booking? GetById(int id)
    {
        int index = GetBucket(id);
        Node? current = _buckets[index];
        while (current != null)
        {
            if (current.Data.BookingID == id)
            {
                return current.Data;
            }
            else
            {
                current = current.Next;
            }
        }
        return null;
    }

    public void Remove(int id)
    {
        int index = GetBucket(id);
        Node? current = _buckets[index];
        Node? previous = null;
        while (current != null)
        {
            if (current.Data.BookingID == id)
            {
                if (previous == null)
                {
                    _buckets[index] = current.Next;
                    return;
                }
                if (previous != null)
                {
                    previous.Next = current.Next;
                    return;
                }
            }
            else
            {
                previous = current;
                current = current.Next;
            }
        }
    }
}