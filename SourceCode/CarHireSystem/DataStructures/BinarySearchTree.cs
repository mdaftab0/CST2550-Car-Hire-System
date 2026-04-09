using CarHireSystem.Models;
using System.Threading;

namespace CarHireSystem.DataStructures;

public class BinarySearchTree : IDisposable
{
    private class Node
    {
        public Car Data { get; set; }
        public Node? Left { get; set; }
        public Node? Right { get; set; }

        public Node(Car car) => Data = car;
    }

    private Node? _root;
    
    private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

    public void Insert(Car car)
    {
        if (car == null) return;

        _lock.EnterWriteLock();
        try
        {
            if (_root == null)
            {
                _root = new Node(car);
                return;
            }

            Node current = _root;
            while (true)
            {
                if (car.PricePerDay < current.Data.PricePerDay)
                {
                    if (current.Left == null) { current.Left = new Node(car); break; }
                    current = current.Left;
                }
                else
                {
                    if (current.Right == null) { current.Right = new Node(car); break; }
                    current = current.Right;
                }
            }
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public void Clear()
    {
        _lock.EnterWriteLock();
        try
        {
            _root = null;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public CarArray SearchByPriceRange(decimal min, decimal max)
    {
        _lock.EnterReadLock();
        try
        {
            CarArray results = new CarArray();
            if (min > max) return results;
            
            SearchRecursive(_root, min, max, results);
            return results;
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    private void SearchRecursive(Node? node, decimal min, decimal max, CarArray results)
    {
        if (node == null) return;

        if (node.Data.PricePerDay > min)
            SearchRecursive(node.Left, min, max, results);

        if (node.Data.PricePerDay >= min && node.Data.PricePerDay <= max)
            results.Add(node.Data);

        if (node.Data.PricePerDay < max)
            SearchRecursive(node.Right, min, max, results);
    }

    public void Dispose()
    {
        _lock.Dispose();
    }
}
