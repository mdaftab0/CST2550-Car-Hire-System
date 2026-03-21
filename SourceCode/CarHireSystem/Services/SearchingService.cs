namespace CarHireSystem.Services;
using CarHireSystem.DataStructures;
public class SearchService
{
    private BinarySearchTree _bst;
    
    public SearchService(BinarySearchTree bst){
        _bst = bst;
    }
}