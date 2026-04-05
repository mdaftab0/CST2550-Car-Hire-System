using NUnit.Framework;
using CarHireSystem.DataStructures;
using CarHireSystem.Models;

namespace CarHireSystem.Tests;

[TestFixture]
public class BSTTests
{
    private BinarySearchTree _bst = null!;

    [SetUp]
    public void SetUp()
    {
        _bst = new BinarySearchTree();
    }

    [Test]
    public void Insert_SingleCar_CanBeFoundByPriceRange()
    {
        var car = new Car(1, "Toyota", "Corolla", "AB12CDE", 35.00m, 5);
        _bst.Insert(car);

        var results = _bst.SearchByPriceRange(30m, 40m);

        Assert.That(results.Count, Is.EqualTo(1));
        Assert.That(results.Get(0).Id, Is.EqualTo(1));
    }

    [Test]
    public void Insert_MultipleCars_AllStoredCorrectly()
    {
        _bst.Insert(new Car(1, "Toyota", "Corolla", "AB12CDE", 35.00m, 5));
        _bst.Insert(new Car(2, "BMW", "X5", "XY99ZZZ", 95.00m, 5));
        _bst.Insert(new Car(3, "Ford", "Fiesta", "FD21ABC", 28.00m, 5));

        var results = _bst.SearchByPriceRange(0m, 200m);

        Assert.That(results.Count, Is.EqualTo(3));
    }

    [Test]
    public void SearchByPriceRange_ReturnsCorrectCars()
    {
        _bst.Insert(new Car(1, "Toyota", "Corolla", "AB12CDE", 35.00m, 5));
        _bst.Insert(new Car(2, "BMW", "X5", "XY99ZZZ", 95.00m, 5));
        _bst.Insert(new Car(3, "Ford", "Fiesta", "FD21ABC", 28.00m, 5));
        _bst.Insert(new Car(4, "Mercedes", "C-Class", "MC55DEF", 120.00m, 5));

        var results = _bst.SearchByPriceRange(30m, 100m);

        Assert.That(results.Count, Is.EqualTo(2));
        var ids = Enumerable.Range(0, results.Count).Select(i => results.Get(i).Id).ToArray();
        Assert.That(ids, Does.Contain(1)); // £35
        Assert.That(ids, Does.Contain(2)); // £95
    }

    [Test]
    public void SearchByPriceRange_NoMatches_ReturnsEmptyArray()
    {
        _bst.Insert(new Car(1, "Toyota", "Corolla", "AB12CDE", 35.00m, 5));
        _bst.Insert(new Car(2, "BMW", "X5", "XY99ZZZ", 95.00m, 5));

        var results = _bst.SearchByPriceRange(200m, 500m);

        Assert.That(results.Count, Is.EqualTo(0));
    }

    [Test]
    public void SearchByPriceRange_ExactBoundaryValues_IncludesBoundaryCars()
    {
        _bst.Insert(new Car(1, "Toyota", "Corolla", "AB12CDE", 35.00m, 5));
        _bst.Insert(new Car(2, "BMW", "X5", "XY99ZZZ", 95.00m, 5));
        _bst.Insert(new Car(3, "Ford", "Fiesta", "FD21ABC", 28.00m, 5));

        // Exact boundaries: min = 28, max = 35 — should include both boundary cars
        var results = _bst.SearchByPriceRange(28.00m, 35.00m);

        Assert.That(results.Count, Is.EqualTo(2));
        var ids = Enumerable.Range(0, results.Count).Select(i => results.Get(i).Id).ToArray();
        Assert.That(ids, Does.Contain(1)); // exactly £35
        Assert.That(ids, Does.Contain(3)); // exactly £28
    }
}
