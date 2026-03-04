# Car Class Design
Inspire Car Hire Management System

---

## 1. Introduction

The Car class represents the main entity of the Inspire Car Hire Management System.

Each car object will store detailed information about a vehicle available for rental. 

The Car objects will be stored inside a custom Binary Search Tree (BST), where CarID will act as the unique key.

---

## 2. Purpose of the Car Class

The Car class is responsible for:

- Storing vehicle information
- Tracking availability status
- Providing data for display and search operations
- Supporting hire and return processes

---

## 3. Attributes of Car Class

Each Car object will contain the following attributes:

- CarID (int) – Unique identifier
- Brand (string)
- Model (string)
- Category (string)  
  (Small, Medium, SUV, Luxury, etc.)
- PricePerDay (double)
- FuelType (string)  
  (Petrol, Diesel, Electric)
- Transmission (string)  
  (Automatic, Manual)
- Seats (int)
- Kilometers (string)  
  (Limited / Unlimited)
- IsAvailable (bool)

---

## 4. Why These Attributes Were Chosen

These attributes were selected based on system requirements:

- Category allows filtering by car type.
- PricePerDay supports cost calculation.
- FuelType and Transmission support customer preferences.
- Seats allow selection based on family size.
- Kilometers supports rental conditions.
- IsAvailable tracks rental status.

This ensures the system reflects real-world car rental functionality.

---

## 5. Methods of Car Class

The Car class will include basic methods:

- Constructor → to initialise car details
- DisplayInfo() → to print car details
- MarkAsRented() → sets IsAvailable to false
- MarkAsReturned() → sets IsAvailable to true

---

## 6. Relationship with BST

The Car object will be stored inside BST nodes.

BST comparison will be done using:

CarID

This ensures:

- Efficient searching
- Unique car identification
- Proper ordering of vehicles

---

## 7. Example Structure (Conceptual)

Car
    CarID = 101
    Brand = "Toyota"
    Model = "Corolla"
    Category = "Medium"
    PricePerDay = 80
    FuelType = "Petrol"
    Transmission = "Automatic"
    Seats = 5
    Kilometers = "Unlimited"
    IsAvailable = true

---

## 8. Conclusion

The Car class is the core entity of the system.

It stores all necessary vehicle information and integrates with the Binary Search Tree to support search, insertion, deletion, and display operations.

This design ensures modularity, clarity, and maintainability of the system.
