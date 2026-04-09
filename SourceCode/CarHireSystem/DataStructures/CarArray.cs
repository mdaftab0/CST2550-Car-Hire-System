using System.Collections;
using CarHireSystem.Models;

namespace CarHireSystem.DataStructures;

public class CarArray : IEnumerable<Car>
{
    private Car[] _items;
    private int _count;

    public int Count => _count;
    public int Capacity => _items.Length;

    public CarArray(int initialCapacity = 10)
    {
        _items = new Car[initialCapacity];
        _count = 0;
    }

    public void Add(Car car)
    {
        if (car == null) throw new ArgumentNullException(nameof(car));

        if (_count == _items.Length)
        {
            EnsureCapacity();
        }
        _items[_count++] = car;
    }

    private void EnsureCapacity()
    {
        int newSize = _items.Length == 0 ? 4 : _items.Length * 2;
        Car[] bigger = new Car[newSize];
        Array.Copy(_items, 0, bigger, 0, _count);
        _items = bigger;
    }

    // Indexer: allows array-like access: myCarArray[0]
    public Car this[int index]
    {
        get
        {
            if (index < 0 || index >= _count)
                throw new IndexOutOfRangeException($"Index {index} is out of bounds.");
            return _items[index];
        }
    }

    // Allows the use of foreach (car in carArray)
    public IEnumerator<Car> GetEnumerator()
    {
        for (int i = 0; i < _count; i++)
        {
            yield return _items[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
