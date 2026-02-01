# ASP.NET Core Web API Master Features

## Overview
NetWebApiMasterFeatures is a modular, scalable, and production-ready ASP.NET Core Web API project designed for rapid enterprise development. It demonstrates best practices in architecture, security, logging, and maintainability, making it an ideal starting point for robust backend services.

## Tech Stack
- **.NET 10 (ASP.NET Core Web API)**
- **Entity Framework Core** (Data Access)
- **OpenAPI/Swagger** (API Documentation)
- **MediatR** (CQRS/Use Cases)
- **Serilog** (Logging)
- **xUnit** (Testing)

## Folder Structure
```
NetWebApiMasterFeatures/
├── API/                # API layer (controllers, middleware, filters)
├── Application/        # Application logic (DTOs, use cases, interfaces)
├── Domain/             # Domain models, entities, value objects
├── Infrastructure/     # Data access, repositories, external services
├── tests/              # Integration and unit tests
└── README.md           # Project documentation
```

## Running the Project Locally
1. **Clone the repository:**
   ```sh
   git clone https://github.com/sachinWebsite/NetCoreWebApi.git
   cd NetCoreWebApi
   ```
2. **Restore dependencies:**
   ```sh
   dotnet restore
   ```
3. **Update environment variables:**
   - Copy `API/appsettings.json` to `API/appsettings.Development.json` and adjust as needed.
   - Set secrets using [dotnet user-secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets):
     ```sh
     cd API
     dotnet user-secrets init
     dotnet user-secrets set "Key" "Value"
     ```
4. **Run the API:**
   ```sh
   dotnet run --project ./API/API.csproj
   ```
5. **Access Swagger UI:**
   - Navigate to [http://localhost:5115/swagger](http://localhost:5115/swagger)

## Environment Configuration
- **appsettings.json**: Base configuration
- **appsettings.Development.json**: Local development overrides
- **User secrets**: For sensitive data (never commit secrets)
- **Environment variables**: For deployment-specific settings

## API Documentation
- **Swagger/OpenAPI** is enabled by default for interactive API documentation and testing.
- Access via `/swagger` endpoint when running locally.

## Best Practices Followed
- **Clean Architecture**: Separation of concerns across API, Application, Domain, and Infrastructure layers
- **Dependency Injection**: Built-in .NET Core DI
- **Centralized Exception Handling**: Global filters and middleware
- **Request/Response Logging**: Middleware for traceability
- **Validation**: Model and request validation filters
- **Environment-based Configuration**: Secure and flexible
- **Unit & Integration Testing**: Test project scaffolded
- **.gitignore**: Prevents committing build artifacts, secrets, and user-specific files

---

> **Note:**
> - Never commit secrets, user files, or environment-specific configs.
> - For production, review security, logging, and error handling settings.
