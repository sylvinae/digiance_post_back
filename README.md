# ASP.NET Core Web API with PostgreSQL

This is an ASP.NET Core Web API project that uses PostgreSQL as the database. The project follows a modular architecture and supports role-based authentication.

## Prerequisites

Ensure the following tools are installed:

1. **.NET SDK** (version 8.0 or later)
2. **PostgreSQL** (version 17 or later)
3. **Command Line Tools**
4. **Dotnet CLI**

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/sylvinae/digiance_post_back.git
cd digiance_post_back
```

### 2. Update the Connection String

Edit the `appsettings.json` file in the project to include your PostgreSQL connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=8888;Database=post;Username=postgres;Password=root"
}
```

Alternatively, use environment variables to avoid hardcoding sensitive details:

```bash
export DefaultConnection="Host=localhost;Port=8888;Database=post;Username=postgres;Password=root"
```

### 3. Apply Migrations

Run the following commands to apply database migrations:

```bash
# Navigate to the project directory
cd <project-folder>

# Add the migrations (if not already present)
dotnet ef migrations add InitialCreate

# Apply the migrations to the database
dotnet ef database update
```

> **Note**: Ensure you have set up the correct database schema in PostgreSQL before running the migration.

### 4. Run the Application

To start the API, run:

```bash
dotnet run
```

The API will be available at `https://localhost:7052/api`.

## API Documentation

The API includes Swagger documentation. Once the API is running, access it at:

- `https://localhost:7052/swagger/index.html`

## Common Commands

### Add a Migration

```bash
dotnet ef migrations add <MigrationName>
```

### Remove the Last Migration

```bash
dotnet ef migrations remove
```

### Update the Database

```bash
dotnet ef database update
```

