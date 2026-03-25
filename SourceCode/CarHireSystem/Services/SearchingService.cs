namespace CarHireSystem.Services;
using CarHireSystem.DataStructures;
public class SearchService
{
    private BinarySearchTree _bst;
    
    public SearchService(BinarySearchTree bst){
        _bst = bst;
    }

    public CarArray SearchByPriceRange(decimal min, decimal max)
    {
        return _bst.SearchByPriceRange(min, max);
    }
}