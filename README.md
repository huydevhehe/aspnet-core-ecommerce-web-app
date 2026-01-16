# 🛒 ASP.NET Core E-commerce Web Application

This is a full-stack E-commerce web application developed using ASP.NET Core MVC, designed to simulate a real-world online shopping system. The project focuses on backend architecture, clean code organization, and common business workflows typically used in production systems.

The application allows users to browse products, manage a shopping cart, place orders, and receive email notifications. It follows the MVC (Model–View–Controller) architecture and applies Repository Pattern and Service Layer to ensure separation of concerns, scalability, and maintainability.

✨ Main Features
- User authentication and authorization
- Product listing and product detail pages
- Shopping cart management
- Order creation and order management workflow
- Email notification using SMTP
- Secure configuration handling (no sensitive data committed)
- Clean backend architecture using Repository and Service layers

🛠️ Technology Stack
- Language: C#
- Framework: ASP.NET Core MVC
- Database: SQL Server
- Data Access: Entity Framework Core
- Architecture: MVC, Repository Pattern, Service Layer
- Frontend: Razor Views, HTML, CSS, JavaScript
- Other: SMTP Email Integration

🗂️ Project Structure
lab1/
├── Controllers/        Handle HTTP requests and routing
├── Models/             Domain and data models
├── Views/              Razor UI views
├── Repositories/       Data access layer
├── Services/           Business logic layer
├── Utilities/          Helper and utility classes
├── wwwroot/            Static assets (CSS, JS, images)
└── Program.cs          Application entry point

WEB.sln                 Solution file  
Data SQL.sql            Database schema and initial data  
README.md  
.gitignore  

▶️ How to Run the Project (Local)

Requirements:
- .NET SDK (ASP.NET Core)
- SQL Server
- Visual Studio or VS Code

Steps:
1. Clone the repository  
   git clone https://github.com/huydevhehe/aspnet-core-ecommerce-web-app.git

2. Open the solution file  
   WEB.sln

3. Restore NuGet packages  

4. Configure database connection  
   Update the connection string in appsettings.json

5. Create database  
   Run the SQL script:  
   Data SQL.sql

6. Run the application  
   dotnet run

7. Open browser and access  
   https://localhost:xxxx

🔐 Security
Sensitive information such as email credentials or secrets is not included in this repository. Environment-specific configuration should be provided using appsettings.Development.json or environment variables. This repository is safe to be public.

📚 Project Information
Type: Academic / Personal Project  
Domain: E-commerce Web Application  
Purpose: Demonstrate backend web development skills using ASP.NET Core MVC

👤 Author
Nguyen Quoc Huy  
GitHub: https://github.com/huydevhehe
