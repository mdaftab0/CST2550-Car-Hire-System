# Binary Search Tree (BST) Design

## Purpose
The Binary Search Tree will store Car objects using CarID as the key.

Each node in the tree will contain:
- Car object
- Left child reference
- Right child reference

---

## Insert Operation (Pseudo Code)

FUNCTION Insert(node, car)

    IF node is NULL
        CREATE new node with car
        RETURN new node

    IF car.CarID < node.Car.CarID
        node.Left = Insert(node.Left, car)

    ELSE IF car.CarID > node.Car.CarID
        node.Right = Insert(node.Right, car)

    RETURN node

---

## Search Operation (Pseudo Code)

FUNCTION Search(node, carID)

    IF node is NULL
        RETURN NULL

    IF carID == node.Car.CarID
        RETURN node

    IF carID < node.Car.CarID
        RETURN Search(node.Left, carID)

    ELSE
        RETURN Search(node.Right, carID)

---

## Time Complexity

Best Case: O(log n)  
Average Case: O(log n)  
Worst Case: O(n)
