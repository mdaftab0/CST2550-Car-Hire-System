# Menu System Design
Inspire Car Hire Management System

---

## 1. Introduction

The Inspire Car Hire Management System will use a console-based menu system.

The menu system allows different users (Admin, Staff, Customer) to access specific system functionalities based on their role.

This design ensures structured navigation and controlled access.

---

## 2. Main Menu (Role Selection)

When the program starts, the following menu will be displayed:

1. Admin
2. Staff
3. Customer
4. Exit

The user selects their role to access the corresponding menu.

---

## 3. Admin Menu

Admin has full control over car records.

Admin Menu Options:

1. Add Car
2. Remove Car
3. View All Cars
4. Back to Main Menu

Functions:

- Add Car → Inserts a new Car into the BST.
- Remove Car → Deletes a Car from the BST using CarID.
- View All Cars → Displays all cars using Inorder Traversal.

---

## 4. Staff Menu

Staff manages rental operations.

Staff Menu Options:

1. View Available Cars
2. Hire Car
3. Return Car
4. Back to Main Menu

Functions:

- View Available Cars → Displays only cars where IsAvailable = true.
- Hire Car → Marks selected car as unavailable.
- Return Car → Marks car as available again.

---

## 5. Customer Menu

Customer has limited access to viewing and searching.

Customer Menu Options:

1. View Cars
2. Search Car by ID
3. Back to Main Menu

Functions:

- View Cars → Displays available cars.
- Search Car → Searches BST using CarID.

---

## 6. Navigation Logic (Pseudo Flow)

Program Start
    ↓
Display Main Menu
    ↓
User selects role
    ↓
Display corresponding menu
    ↓
Perform selected operation
    ↓
Return to role menu or main menu

---

## 7. Error Handling

- Invalid input will prompt user to try again.
- If CarID not found, display appropriate message.
- Duplicate CarID insertion will be prevented.

---

## 8. Integration with BST

The menu system interacts directly with:

- Insert operation (Admin)
- Delete operation (Admin)
- Search operation (Staff & Customer)
- Inorder traversal (Display cars)

This ensures the Binary Search Tree remains central to the system architecture.

---

## 9. Conclusion

The menu system provides structured role-based access to system functionalities.

It ensures:

- Clear separation of responsibilities
- Secure modification of car records
- User-friendly navigation
- Proper integration with the custom Binary Search Tree

This design supports modular and scalable system implementation.
