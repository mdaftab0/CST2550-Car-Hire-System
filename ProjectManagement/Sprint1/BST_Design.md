# Binary Search Tree (BST) Design
Inspire Car Hire Management System

---

## 1. Introduction

The Inspire Car Hire Management System requires an efficient way to store, search, insert, and remove car records. 

To achieve this, a **custom Binary Search Tree (BST)** will be implemented instead of using built-in data structures.

The BST will store **Car objects**, using the CarID as the unique key for ordering.

This ensures efficient searching and management of vehicles in the system.

## 2. Why Binary Search Tree?

The BST was chosen because:

- It provides faster searching compared to linear lists.
- It allows efficient insertion and deletion.
- It supports ordered data storage.
- It demonstrates understanding of custom data structures (coursework requirement).

Time Complexity (Average Case):
- Search: O(log n)
- Insert: O(log n)
- Delete: O(log n)

Worst Case (Unbalanced Tree):
- Search: O(n)
- Insert: O(n)
- Delete: O(n)

The worst case occurs when the tree becomes skewed 
(e.g., inserting already sorted data), causing it to behave like a linked list.

---

## 3. Structure of BST Node

Each node in the Binary Search Tree will contain:

- Car object
- Reference to Left child
- Reference to Right child

Conceptual Structure:

Node
    Car Data
    Left → Node
    Right → Node


## 4. Key Used for Comparison

The BST will use:

CarID (integer)

Rules:
- If new CarID < current node CarID → go LEFT
- If new CarID > current node CarID → go RIGHT
- Duplicate CarIDs are not allowed

---

## 5. Operations of the BST

### 5.1 Insert Operation

Purpose:
Used by Admin to add new cars to the system.

Logic:
- If tree is empty → new node becomes root.
- If CarID is smaller → insert in left subtree.
- If CarID is larger → insert in right subtree.
- If duplicate → reject insertion.

Pseudo Code:

FUNCTION Insert(node, car)

    IF node IS NULL
        CREATE new node with car
        RETURN new node

    IF car.CarID < node.Car.CarID
        node.Left = Insert(node.Left, car)

    ELSE IF car.CarID > node.Car.CarID
        node.Right = Insert(node.Right, car)

    ELSE
        PRINT "Duplicate CarID not allowed"

    RETURN node

---

### 5.2 Search Operation

Purpose:
Used by:
- Customer to search for a car
- Staff to check availability
- Admin to verify records

Logic:
- Compare CarID
- Traverse left or right accordingly
- Return car if found

Pseudo Code:

FUNCTION Search(node, carID)

    IF node IS NULL
        RETURN NULL

    IF carID == node.Car.CarID
        RETURN node

    IF carID < node.Car.CarID
        RETURN Search(node.Left, carID)

    ELSE
        RETURN Search(node.Right, carID)

---

### 5.3 Delete Operation

Purpose:
Used by Admin to remove a car from the system.

Three Cases:

1. Node has no children → simply remove.
2. Node has one child → replace with child.
3. Node has two children → replace with inorder successor.

Pseudo Code (Simplified):

FUNCTION Delete(node, carID)

    IF node IS NULL
        RETURN NULL

    IF carID < node.Car.CarID
        node.Left = Delete(node.Left, carID)

    ELSE IF carID > node.Car.CarID
        node.Right = Delete(node.Right, carID)

    ELSE
        IF node has no children
            RETURN NULL

        IF node has one child
            RETURN child node

        IF node has two children
            successor = FindMin(node.Right)
            node.Car = successor.Car
            node.Right = Delete(node.Right, successor.CarID)

    RETURN node

---

## 6. Traversal Method

To display cars (View Cars option), the system will use:

Inorder Traversal

Why?
Because it prints cars in sorted order of CarID.

Pseudo Code:

FUNCTION Inorder(node)

    IF node IS NOT NULL
        Inorder(node.Left)
        PRINT node.Car
        Inorder(node.Right)

---

## 7. Integration with System Roles

Admin:
- Insert Car
- Delete Car
- View All Cars (Inorder Traversal)

Staff:
- Search Car
- Mark car as unavailable (IsAvailable = false)

Customer:
- Search Car
- View Available Cars

The BST ensures efficient access for all roles.

---

## 8. Limitations

If the BST becomes unbalanced (e.g., sorted insertion),
performance may degrade to O(n).

Future Improvement:
Use a self-balancing tree (AVL or Red-Black Tree).

---

## 9. Conclusion

The Binary Search Tree is a suitable data structure for managing car records in the Inspire Car Hire Management System.

It provides:
- Efficient searching
- Structured storage
- Logical organisation
- Academic demonstration of custom data structure implementation

This design satisfies the coursework requirement for implementing and analysing a custom data structure.
