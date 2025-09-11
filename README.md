# EBook - Digital Library System

 <!-- Replace with a URL to a nice screenshot of your dashboard -->

**EBook** is a comprehensive full-stack web application designed to be a modern digital library platform. It connects vendors (bookstores, libraries) with customers (readers), creating a robust marketplace for buying and borrowing both physical and digital books. This project is built with a professional **Clean Architecture** approach using the .NET stack.

**Live Demo:** `[Link to your live demo if you have one]`

---

## 📚 Project Vision

The goal of this project is to create a scalable, maintainable, and secure platform that serves two primary user groups:
*   **Vendors:** Bookstores, publishers, and individual sellers who need a platform to manage their inventory and reach a wider audience.
*   **Customers:** Readers and students looking for a centralized place to discover, purchase, and borrow books in various formats.

---

## ✨ Key Features

### For Customers (Readers)
- **User Authentication:** Secure registration and login system using ASP.NET Core Identity.
- **Book Discovery:** Advanced search and filtering to find books by title, author, or genre.
- **Flexible Purchasing Options:**
  - **Buy PDF:** Instant download after purchase.
  - **Buy Physical Book:** Shipped to the user's address.
- **Digital Borrowing System:** Borrow PDF versions of books for a limited time.
- **Shopping Cart & Checkout:** A seamless e-commerce experience.
- **Personal Dashboard:** Track orders, manage borrowed books, and view personal activity.

### For Admins (Platform Management)
- **Dynamic Dashboard:** A comprehensive overview of key metrics, including user growth, sales performance, and recent activities.
- **User Management:**
  - **Customer Management:** View, activate, and ban customer accounts.
  - **Vendor Management:** A dedicated system to review and approve/reject new vendor applications.
- **Content Management:** Full control over all books on the platform, with the ability to edit or remove content.
- **Financial Oversight:** Track orders and transactions.

---

## 🏛️ Architecture: Built with Clean Architecture

This project is built following the principles of **Clean Architecture**. This architectural pattern ensures a clear separation of concerns, making the application highly maintainable, testable, and scalable.

The solution is divided into three primary layers:

### 1. `Digital-Library.Core`
This is the heart of the application. It contains the core business logic, models, and interfaces.
- **Models:** Entities like `User`, `Book`, `Vendor`, `Order`.
- **Interfaces (Contracts):** Defines the contracts for services (e.g., `IAuthService`, `IBookService`, `IDashboardService`) without concerning itself with implementation details.
- **Key Principle:** This layer has **zero dependencies** on external frameworks like databases or UI.

### 2. `Digital-Library.Infrastructure`
This layer implements the contracts defined in the Core layer. It handles all external concerns.
- **Data Access:** Contains the `DbContext` for Entity Framework Core, managing all database interactions.
- **Services:** Concrete implementations of the interfaces (e.g., `EmailSender` for sending emails via SMTP, `BookService` for database operations on books).
- **Key Principle:** Isolates technical details. We can swap out the database or email provider by changing only this layer.

### 3. `Digital-Library.AdminPanel` (Presentation Layer)
This is the user-facing ASP.NET Core MVC application.
- **Controllers:** Thin controllers that receive user requests and orchestrate actions by calling services from the Core/Infrastructure layers.
- **Views:** Dynamic web pages built with Razor and styled with **Tailwind CSS**.
- **ViewModels:** Custom models designed specifically for the views to ensure security and separation of concerns.
- **`Program.cs`:** The composition root where all services and dependencies are wired up using Dependency Injection.

 <!-- It would be great to create a simple diagram for this -->

---

## 🛠️ Technology Stack

| Area | Technology |
| :--- | :--- |
| **Backend** | C#, .NET 8, ASP.NET Core MVC, Entity Framework Core, ASP.NET Core Identity |
| **Frontend** | HTML5, Tailwind CSS, JavaScript, jQuery, Chart.js |
| **Database** | SQL Server (designed to be EF Core provider-agnostic) |
| **Architecture** | Clean Architecture, Dependency Injection (DI) |
| **Testing** | xUnit, Moq (for unit testing) |
| **CI/CD** | GitHub Actions |

---

## 🚀 Getting Started

Follow these instructions to get a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or another compatible database)
- A C# IDE like [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/).

### Installation & Setup

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/yousefelsharkawy26/EBook.git
    cd EBook/Digital-Library
    ```

2.  **Configure the database connection:**
    - Open `Digital-Library.AdminPanel/appsettings.json`.
    - Find the `ConnectionStrings` section.
    - Update the `DefaultConnection` string to point to your local database instance.

3.  **Configure email settings (optional):**
    - In `appsettings.json`, update the `EmailSettings` section with your SMTP server details (e.g., a Gmail App Password).

4.  **Apply database migrations:**
    - Open the Package Manager Console in Visual Studio (or use the command line).
    - Make sure the `Digital-Library.AdminPanel` project is set as the default project.
    - Run the following command to create and apply the database schema:
    ```bash
    Update-Database
    ```

5.  **Run the application:**
    - Build and run the `Digital-Library.AdminPanel` project from your IDE (e.g., press F5 in Visual Studio).
    - The application should now be running locally.

---

## 📈 CI/CD Pipeline

This repository is configured with a **GitHub Actions** workflow located at `.github/workflows/dotnet.yml`. This pipeline automates the following process on every push or pull request to the `main` branch:
1.  **Restore** NuGet dependencies.
2.  **Build** the entire solution in Release mode.
3.  **Run** all unit tests to ensure code quality.

This ensures that the main branch always contains stable, working code.

---

## 🤝 Contributing

Contributions, issues, and feature requests are welcome! Feel free to check the [issues page](https://github.com/yousefelsharkawy26/EBook/issues).

## 📄 License

This project is licensed under the MIT License - see the `LICENSE.md` file for details.
