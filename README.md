# ICMarkets WEB API .NET Developer Project

## Description
This project is a .NET Web API for storing and retrieving blockchain data from BlockCypher API.  
It uses Clean Architecture, CQRS, MediatR, AutoMapper, and Entity Framework Core.

## Features
- Fetch blockchain data (BTC, ETH, LTC, DASH) from BlockCypher API and store it in a database  
- Retrieve stored blockchain history data  
- API documentation with Swagger  
- Health checks endpoint  
- CORS policy allowing all origins  
- Supports SQLite and InMemory database for testing  
- Integration tests with NUnit and WebApplicationFactory  

## Requirements
- .NET 6 or higher  
- SQLite (for production)  
- Visual Studio 2022 or VS Code  

## Setup and Running

1. Clone the repository:
   ```
   git clone https://github.com/your-repo/ICMarkets.git
   cd ICMarkets
   ```

2. Restore NuGet packages:
   ```
   dotnet restore
   ```

3. Update the connection string in `appsettings.json` if needed (default uses SQLite):
   ```json
   "ConnectionStrings": {
       "DefaultConnection": "Data Source=blockchains.db"
   }
   ```

4. Run database migrations (if using relational database):
   ```
   dotnet ef database update
   ```

5. Run the API:
   ```
   dotnet run --project ICMarkets.API
   ```

6. The API will be available at `https://localhost:5001` (or `http://localhost:5000`).

7. Swagger UI for API docs is available at `/swagger`.

## Running Tests

Integration tests are implemented with NUnit. To run tests:

```
dotnet test ICMarkets.IntegrationTests
```

Tests use an InMemory database provider.

## Project Structure

- `ICMarkets.API` - Web API project  
- `ICMarkets.Application` - Application layer with CQRS, MediatR, DTOs, and business logic  
- `ICMarkets.Domain` - Domain entities and enums  
- `ICMarkets.Persistence` - EF Core DbContext and repository implementations  
- `ICMarkets.Infrastructure` - Infrastructure services (e.g., HTTP clients)  
- `ICMarkets.IntegrationTests` - Integration tests for the API  

## Notes

- The API supports asynchronous calls and cancellation tokens.  
- The integration tests spin up a test server with an InMemory database for isolation.  
- Enum serialization uses `JsonStringEnumConverter` with case-insensitive matching.  

---

If you have questions or issues, please open an issue or contact the maintainer.
