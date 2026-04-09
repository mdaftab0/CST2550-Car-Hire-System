namespace CarHireSystem.DataStructures;
using CarHireSystem.Models;

public class HashTable
{
    private class Node
    {
        public Booking Data;
        public Node? Next;
        public Node(Booking booking) => Data = booking;
    }

    private Node?[] _buckets;
    private int _count;
    private const double LoadFactorThreshold = 0.75;
    private readonly object _lock = new object();

    public HashTable(int initialCapacity = 16)
    {
        _buckets = new Node?[initialCapacity];
    }

    private int GetBucketIndex(int key, int bucketCount)
    {
        // Use bitwise AND or Math.Abs for safe indexing
        return Math.Abs(key.GetHashCode()) % bucketCount;
    }

    public void Insert(Booking booking)
    {
        lock (_lock)
        {
            // Check if we need to grow the table to keep it fast
            if ((double)_count / _buckets.Length >= LoadFactorThreshold)
            {
                Resize();
            }

            int index = GetBucketIndex(booking.BookingID, _buckets.Length);
            
            // Avoid duplicate IDs
            Node? current = _buckets[index];
            while (current != null)
            {
                if (current.Data.BookingID == booking.BookingID)
                {
                    current.Data = booking; // Update existing
                    return;
                }
                current = current.Next;
            }

            // Insert at the head of the chain (O(1))
            Node newNode = new Node(booking) { Next = _buckets[index] };
            _buckets[index] = newNode;
            _count++;
        }
    }

    public Booking? GetById(int id)
    {
        lock (_lock)
        {
            int index = GetBucketIndex(id, _buckets.Length);
            Node? current = _buckets[index];
            while (current != null)
            {
                if (current.Data.BookingID == id) return current.Data;
                current = current.Next;
            }
            return null;
        }
    }

    public bool Remove(int id)
    {
        lock (_lock)
        {
            int index = GetBucketIndex(id, _buckets.Length);
            Node? current = _buckets[index];
            Node? previous = null;

            while (current != null)
            {
                if (current.Data.BookingID == id)
                {
                    if (previous == null) _buckets[index] = current.Next;
                    else previous.Next = current.Next;
                    
                    _count--;
                    return true;
                }
                previous = current;
                current = current.Next;
            }
            return false;
        }
    }

    private void Resize()
    {
        int newSize = _buckets.Length * 2;
        Node?[] newBuckets = new Node?[newSize];

        // Rehash every single item into the new larger array
        for (int i = 0; i < _buckets.Length; i++)
        {
            Node? current = _buckets[i];
            while (current != null)
            {
                Node? next = current.Next; // Save reference
                
                int newIndex = GetBucketIndex(current.Data.BookingID, newSize);
                current.Next = newBuckets[newIndex];
                newBuckets[newIndex] = current;

                current = next;
            }
        }
        _buckets = newBuckets;
    }

    public int Count => _count;
}
