# Wallet API

Wallet API is a modern and robust RESTful API for creating digital wallets, depositing, withdrawing, and transferring funds between wallets. The project is designed with Clean Architecture principles for sustainable, testable, and scalable software development.

## ❓ Why Wallet API?

This project aims to provide a secure, extensible, and developer-friendly foundation for digital wallet solutions. It can be used for fintech prototypes, payment platforms, or as a learning resource for Clean Architecture in .NET.

## 🗂️ Project Structure

```
src/
├── Cuzdan.Api/           # ASP.NET Core Web API (presentation layer)
├── Cuzdan.Application/   # Application logic, DTOs, services, validation
├── Cuzdan.Domain/        # Core domain models, entities, enums, errors
├── Cuzdan.Infrastructure/# Database, authentication, gateways, repositories
├── Cuzdan.Tests/         # Unit and integration tests
```

## 🏗️ Architecture

The project follows Clean Architecture and consists of the following layers:

- **Domain:** The core of the application. Contains business logic, entities, enums, and domain errors. Has no dependencies on other layers.
- **Application:** Contains application logic and workflows. Includes service interfaces, DTOs, validation rules, and application-specific errors.
- **Infrastructure:** Implements external operations such as database access (Entity Framework Core), third-party service communication (Gateway), and authentication (JWT).
- **Api:** Presentation layer (ASP.NET Core Web API). Handles HTTP requests and delegates to the Application layer.

Additionally, cross-cutting concerns (e.g., logging) are dynamically applied using design patterns like Decorator and Scrutor.

## 🚀 Technologies & Libraries

- **Framework:** .NET 9
- **API:** ASP.NET Core Web API
- **Database:** PostgreSQL
- **ORM:** Entity Framework Core 9
- **Authentication:** JWT
- **Validation:** FluentValidation
- **Logging:** Serilog (Console, File, Seq)
- **Documentation:** Swashbuckle (Swagger)
- **Dependency Injection:** Scrutor
- **Testing:** xUnit, Moq, FluentAssertions
- **Password Hashing:** BCrypt.Net.BCrypt is used for secure password hashing.  
  > Note: BCrypt is intentionally slow by design to increase security against brute-force attacks. This may cause login operations to be slower than typical hash algorithms.

## ✨ Features

- **User Management:** Secure registration, login, and token refresh.
- **Wallet Management:** Create and list wallets with different currencies.
- **Money Operations:** Deposit, withdraw, and instant transfer.
- **Transaction History:** Query transaction history with pagination and filtering.
- **Currency Conversion:** View balances with real-time exchange rates.
- **Payment Webhook:** Mock endpoints for simulating real deposit and withdrawal events.
- **Advanced Logging:** End-to-end tracing with CorrelationId per request.
- **Global Error Handling:** Centralized error catching with standard error responses.

## 🛠️ Setup & Run

### Requirements

- .NET 9 SDK
- PostgreSQL
- Visual Studio 2022 or VS Code

### 1. Clone the Repository

```bash
git clone https://your-repository-url.com
cd cuzdan_test
```

All required NuGet packages are already listed in the `.csproj` files.  
After cloning the repository, simply run:
>
```bash
dotnet restore
```
This will automatically install all dependencies.

### 2. Configure Settings

Update the following fields in `src/Cuzdan.Api/appsettings.json`:

**Database Connection:**
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=cuzdan_db;Username=your_user;Password=your_password;"
}
```

**JWT Settings:**
```json
"Jwt": {
  "Key": "your-super-secret-key-that-is-long-enough",
  "Issuer": "https://localhost:5001",
  "Audience": "https://localhost:5001"
}
```

### 3. Add Initial Migration (if migrations do not exist)

If the `src/Cuzdan.Infrastructure/Migrations` folder does **not** exist, create the initial migration:

```bash
dotnet ef migrations add Init --project src/Cuzdan.Infrastructure --startup-project src/Cuzdan.Api
```

### 4. Add Function And Procedure Migration (if migrations do not exist)

```bash
dotnet ef migrations add FunAndPro --project src/Cuzdan.Infrastructure --startup-project src/Cuzdan.Api
```

After creating the `FunAndPro` migration, copy the SQL code from `src/Cuzdan.Infrastructure/Sql_func_procedure.txt` and paste it into the appropriate section of the newly generated migration file (usually inside the `Up` and `Down` methods). This ensures your functions and procedures are applied to the database during migration.

### 5. Update Database

```bash
dotnet ef database update --project src/Cuzdan.Infrastructure --startup-project src/Cuzdan.Api
```

### 6. Run Project

```bash
dotnet run --project src/Cuzdan.Api
```

## Common Development Commands

Below are useful commands for building, running, and managing the database:

```bash
# Build the solution
dotnet build

# Add initial migration
dotnet ef migrations add Init --project src/Cuzdan.Infrastructure --startup-project src/Cuzdan.Api

# Apply migrations to the database
dotnet ef database update --project src/Cuzdan.Infrastructure --startup-project src/Cuzdan.Api

# Run the API
dotnet run --project src/Cuzdan.Api

# Drop the database (use with caution!)
dotnet ef database drop --force --project src/Cuzdan.Infrastructure --startup-project src/Cuzdan.Api
```

The API will be available at `https://localhost:5001`.

Note that admin password is "admin123"

## 📚 API Usage

You can explore all endpoints and documentation via the Swagger UI:

[https://localhost:5001/swagger](https://localhost:5001/swagger)

## 🧪 Testing

Unit and integration tests are available in the `Cuzdan.Tests` project.



## 📄 License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---
