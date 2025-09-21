🎮 Epic Games Store (C# WinForms)

This project is a desktop-based Epic Games Store clone built using C# WinForms, SQL Server, and Guna.UI2 framework for modern UI styling. It simulates a digital game store where users can browse, add games to cart, purchase them using different payment methods, and track order history. Game owners can also track their sales, commissions, and earnings.



🚀 Features

User Authentication: Register & log in with secure credentials.

Game Browsing: Explore games with images, price, descriptions, and system requirements.

Cart System: Add, remove, and update items in the shopping cart.

Checkout & Payment: Supports multiple payment options (Bkash, Visa, PayPal).

Order Management: Tracks user orders with details and statuses.

Owner Earnings: Game owners receive revenue minus commission.

Commission Tracking: Store commission is automatically calculated and saved.

Reviews: Users can leave ratings and reviews on games.

Coupons: Discounts applied via promotional codes.

Modern UI: Built with Guna.UI2 WinForms for a sleek interface.



🛠️ Tech Stack

Language: C# (.NET Framework)

UI Library: Guna.UI2.WinForms

Database: SQL Server (MSSQL)

IDE: Visual Studio



🗄️ Database Schema

The project uses a relational database with the following tables:

Users: Stores user info (ID, Name, Email, Password, Role, CompanyName, Security Q&A).

Games: Contains game details (Name, Price, Type, Description, Requirements, Quantity, OwnerEmail).

Cart: Each user has one cart linked by UserEmail.

CartItems: Stores games added to the cart with quantity.

Orders: Records user orders with status and total amount.

OrderDetails: Links each order to purchased games and quantities.

PaymentHistory: Tracks payments (method, transaction ID, amount, date, status).

OwnerEarnings: Tracks revenue split between store and game owners.

Commission: Tracks commission per order/game.

Coupons: Promotional discount codes with expiry and creator info.

UserCoupons: Which coupons were used by which user.

Reviews: User reviews with rating and comments.



⚡ Getting Started
Prerequisites

Install Visual Studio with Windows Forms workload.

Install SQL Server (Express or full edition).

Clone this repository:

git clone https://github.com/towhidislam772/Epic-Games-Store-CSharp.git


Restore NuGet packages (for Guna.UI2).

Database Setup

Create a new SQL Server database.

Run the provided schema (from README / DatabaseScripts/).

Update your connection string in Payment.cs, AddToCart.cs, and other DB-related files:

string connectionString = "Data Source=YOUR_SERVER;Initial Catalog=Faw;Integrated Security=True";

Run

Build and run the project in Visual Studio.



📌 Future Improvements

JWT or OAuth-based secure login.

Cloud database hosting (Azure SQL).

Game downloads & license keys.

Admin dashboard for better management.



👨‍💻 Author

Towhid Islam
🔗 GitHub: towhidislam772
