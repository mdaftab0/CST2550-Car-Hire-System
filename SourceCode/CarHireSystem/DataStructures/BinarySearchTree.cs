namespace CarHireSystem.DataStructures;
using CarHireSystem.Models;

public class BinarySearchTree
{
    private class Node
    {
        public Car Data { get; set; }
        public Node? Left { get; set; }
        public Node? Right { get; set; }

        public Node(Car car)
        {
            Data = car;
            Left = null;
            Right = null;
        }
    }

    private Node? _root;

    public void Insert(Car car)
    {
        if (_root == null)
        {
            _root = new Node(car);
        }
        else
        {
            Node current = _root;
            while (true)
            {
                if (car.PricePerDay < current.Data.PricePerDay)
                {
                    if (current.Left == null)
                    {
                        current.Left = new Node(car);
                        return;
                    }
                    else
                    {
                        current = current.Left;
                    }
                }
                else if (car.PricePerDay > current.Data.PricePerDay)
                {
                    if (current.Right == null)
                    {
                        current.Right = new Node(car);
                        return;
                    }
                    else
                    {
                        current = current.Right;
                    }
                }
                else
                {
                    return;
                }
            }
        }
    }

    public CarArray SearchByPriceRange(decimal min, decimal max)
    {
        CarArray results = new CarArray();
        SearchRecursive(_root, min, max, results);
        return results;
    }
    private void SearchRecursive(Node? node, decimal min, decimal max, CarArray results)
    {
        if (node == null)
        {
            return;
        }
        if (min <= node.Data.PricePerDay && node.Data.PricePerDay <= max)
        {
            results.Add(node.Data);
        }
        if (min < node.Data.PricePerDay)
        {
            SearchRecursive(node.Left, min, max, results);
        }
        if (max > node.Data.PricePerDay)
        {
            SearchRecursive(node.Right, min, max, results);
        }
    }
}