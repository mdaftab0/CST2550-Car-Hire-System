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
}