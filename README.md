# ArenaSync

**ArenaSync** is a web-based arena and sports-event management system built with ASP.NET Core Blazor Server (.NET 10). It manages venues, events, teams, attendees, and vendors — with role-based access control, server-side validation, and reporting.

---

## Table of Contents

- [Tech Stack](#tech-stack)
- [Prerequisites](#prerequisites)
- [Setup](#setup)
- [Running the App](#running-the-app)
- [Running Tests](#running-tests)
- [Demo Users](#demo-users)
- [Seeding](#seeding)
- [Troubleshooting](#troubleshooting)

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core Blazor Server (.NET 10) |
| ORM | Entity Framework Core 10 |
| Database | SQL Server / SQL Server Express |
| Auth | ASP.NET Core Identity |
| Testing | xUnit + SQLite in-memory |
| Styling | Bootstrap 5 + custom CSS |

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server Express (or full SQL Server)
- EF Core CLI tools: `dotnet tool install --global dotnet-ef`

---

## Setup

### 1. Clone the repository

```bash
git clone <your-repo-url>
cd EECS447Project
```

### 2. Configure the connection string

Edit `ArenaSync.Web/appsettings.json` to point at your SQL Server instance:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=ArenaSyncDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

For production, set the `ConnectionStrings__DefaultConnection` environment variable instead of editing the file.

### 3. Apply migrations

```bash
dotnet ef database update --project ArenaSync.Web
```

---

## Running the App

```bash
dotnet run --project ArenaSync.Web
```

The app starts at `https://localhost:5119` (or the port shown in the console). On first run in Development mode the database is automatically seeded with demo data.

---

## Running Tests

```bash
dotnet test
```

Tests use an in-memory SQLite database — no SQL Server required. All 73 tests should pass.

---

## Demo Users

These accounts are seeded automatically in Development mode:

| Role | Email | Password |
|---|---|---|
| Admin | admin@arenasync.com | Admin123! |
| Manager | manager@arenasync.com | Manager123! |
| Viewer | viewer@arenasync.com | Viewer123! |

---

## Seeding

The `DbSeeder` runs automatically on startup in the Development environment. It is idempotent — it skips any table that already has rows, so restarting the app will not duplicate data.

To reset to a clean seeded state:

```bash
dotnet ef database drop --project ArenaSync.Web
dotnet run --project ArenaSync.Web
```

---

## Troubleshooting

**"PendingModelChangesWarning" on startup**
Run `dotnet ef migrations add <name> --project ArenaSync.Web` to generate a new migration, then restart.

**FK constraint error on seeding**
Drop and recreate the database (see Seeding section above).

**Cannot connect to SQL Server**
Verify SQL Server Express is running: open `services.msc` and ensure "SQL Server (SQLEXPRESS)" is Started.
