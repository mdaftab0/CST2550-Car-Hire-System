# EasyHire Car Hire System

A web-based car hire management system built with ASP.NET Core (.NET 10), Razor Pages, Entity Framework Core, and Azure SQL. Developed for CST2550.

## Features

- Browse and filter cars by price range (custom Binary Search Tree)
- Book a car with Stripe payment processing
- Return cars and view booking history
- Admin dashboard for fleet and booking management
- AI chatbot powered by Claude API
- Google OAuth and email/password authentication

## Project Structure

```
/SourceCode/CarHireSystem          — Main web application
/SourceCode/CarHireSystem.Tests    — NUnit unit tests
/Database/schema.sql               — Full SQL schema generated from EF Core migrations
/ProjectManagement                 — Sprint documents, meeting minutes, design docs
/Report                            — PDF report
/Video                             — Demo video (MP4)
```

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Access to an Azure SQL Server instance (or any SQL Server)
- A Stripe account (test keys are fine)
- A Claude API key (for the AI chatbot)

## Setup and Run

### 1. Clone the repository

```bash
git clone https://github.com/<your-repo-url>.git
cd CST2550-Car-Hire-System/SourceCode/CarHireSystem
```

### 2. Configure secrets

Open `appsettings.Development.json` and fill in your values:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=<your-server>.database.windows.net;Database=CarHireDb;User Id=<user>;Password=<password>;Encrypt=True;"
  },
  "Stripe": {
    "PublishableKey": "pk_test_...",
    "SecretKey": "sk_test_..."
  },
  "Authentication": {
    "Google": {
      "ClientId": "your-google-client-id",
      "ClientSecret": "your-google-client-secret"
    }
  },
  "Claude": {
    "ApiKey": "your-claude-api-key"
  }
}
```

### 3. Apply database migrations

```bash
dotnet ef database update
```

This creates all tables and is safe to run on a fresh database. The application also seeds 20 cars, the Admin and Customer roles, and a default admin account on first run.

### 4. Run the application

```bash
dotnet run
```

Then open `https://localhost:5001` (or the URL shown in the terminal) in your browser.

### 5. Default admin account

| Field    | Value                  |
|----------|------------------------|
| Email    | admin@easyhire.com     |
| Password | Admin123!              |

## Running the Tests

```bash
cd ../CarHireSystem.Tests
dotnet test
```

Tests cover the custom Binary Search Tree, Hash Table, Car model, and Booking model using NUnit.

## Stripe Test Card

When making a booking, use the following test card — no real payment is taken:

| Field          | Value               |
|----------------|---------------------|
| Card number    | 4242 4242 4242 4242 |
| Expiry         | Any future date     |
| CVC            | Any 3 digits        |

## Custom Data Structures

| Structure       | Location                              | Purpose                                      |
|-----------------|---------------------------------------|----------------------------------------------|
| BinarySearchTree | `DataStructures/BinarySearchTree.cs` | O(log n) car search by price range           |
| HashTable        | `DataStructures/HashTable.cs`        | O(1) average booking storage and retrieval   |
| CarArray         | `DataStructures/CarArray.cs`         | Dynamic array returned by BST search results |
