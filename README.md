# MVCWebApp

## Overview

`MVCWebApp` is an ASP.NET Core MVC application designed for managing courses and enrollments. It provides functionality to create, read, update, and delete (CRUD) courses, view course statistics, and manage student enrollments. The application uses Entity Framework Core (EF Core) for data access and SQL Server as the database.

### Features
- **Course Management**: Add, edit, delete, and view courses with details such as course name, lecturer, start date, tuition fee, and student limit.
- **Enrollment Tracking**: Calculate remaining slots for each course based on enrollments.
- **Statistics Dashboard**: Visualize courses started per month using a chart.
- **Search and Sort**: Filter courses by name or lecturer and sort the list dynamically.
- **Database Migrations**: Use EF Core migrations to manage the database schema and seed initial data.

## Project Structure
- **Controllers/**: Contains MVC controllers (e.g., `CoursesController` for managing courses).
- **Data/**: Includes the `AppDbContext` class for EF Core configuration and database context.
- **Migrations/**: Stores EF Core migration files for database schema changes.
- **Models/**: Defines entity classes (e.g., `Course`, `Enrollment`, `CourseViewModel`).
- **Views/**: Contains Razor views for rendering the UI.
- **wwwroot/**: Holds static files (CSS, JavaScript, images).
- **Program.cs**: Entry point of the application.
- **appsettings.json**: Configuration settings (e.g., database connection string).

## Prerequisites
Before running the project, ensure you have the following installed:
- **.NET 9.0 SDK** (or the version specified in `MVCWebApp.csproj`).
- **SQL Server** (LocalDB or a full SQL Server instance).
- **Visual Studio 2022** (or another IDE like Visual Studio Code with the C# extension).

  ```bash
  dotnet restore
  
## Nuget packages
- **Bootstrap 5** (for better UI):
- **Microsoft.EntityFrameworkCore** (for migrations):
- **Microsoft.AspNetCore.Identity** (for user authentication/authorization):
- **Microsoft.EntityFrameworkCore.SqlServer** (for database interactions):

  ```bash
  dotnet tool install --global dotnet-ef
