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
        _items[_count] = car;
        _count++;
    }

	public int Count => _count;

    public Car Get(int index)
    {
        return _items[index];
    }
}