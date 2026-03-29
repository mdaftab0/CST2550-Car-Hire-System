namespace CarHireSystem.DataStructures;
using CarHireSystem.Models;

public class CarArray
{
    private Car[] _items;
    private int _count;

    public CarArray()
    {
        _items = new Car[10];
        _count = 0;
    }

    public void Add(Car car)
    {
        if (_count == _items.Length)
        {
            var bigger = new Car[_items.Length * 2];
            for (int i = 0; i < _items.Length; i++)
                bigger[i] = _items[i];
            _items = bigger;
        }
        _items[_count] = car;
        _count++;
    }

	public int Count => _count;

    public Car Get(int index)
    {
        return _items[index];
    }
}