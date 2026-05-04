# ArenaSync — Deployment Guide

## Prerequisites

- .NET 10 Runtime (or SDK) on the target server
- SQL Server (any edition) accessible from the server
- A reverse proxy (IIS, nginx, or Caddy) recommended for production

---

## Environment Configuration

Never put production credentials in `appsettings.json`. Use environment variables instead:

```bash
# Connection string
ConnectionStrings__DefaultConnection="Server=prod-sql;Database=ArenaSyncDb;User Id=appuser;Password=secret;TrustServerCertificate=True;"

# ASP.NET Core environment
ASPNETCORE_ENVIRONMENT=Production
```

On Windows (IIS), set these in the Application Pool's environment variables or in `web.config`.  
On Linux, set them in the systemd unit file or in your container's environment.

---

## Production appsettings

`appsettings.Production.json` is committed with safe defaults and placeholder values. It overrides `appsettings.json` when `ASPNETCORE_ENVIRONMENT=Production`.

Key settings to verify before deploying:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "SET_VIA_ENVIRONMENT_VARIABLE"
  }
}
```

---

## Publishing

```bash
dotnet publish ArenaSync.Web/ArenaSync.Web.csproj \
  -c Release \
  -o ./publish \
  --self-contained false
```

Copy the `./publish` folder to the server.

---

## Database Migration on Production

Run migrations against the production database before starting the app:

```bash
dotnet ef database update \
  --project ArenaSync.Web \
  --connection "Server=prod-sql;Database=ArenaSyncDb;..."
```

Or set the connection string as an environment variable and run:

```bash
dotnet ArenaSync.Web.dll --migrate-only
```

The seeder runs only in the Development environment — it will **not** seed production data automatically.

---

## IIS Deployment (Windows)

1. Install the [.NET 10 Hosting Bundle](https://dotnet.microsoft.com/download/dotnet/10.0)
2. Create a new IIS site pointing to the `publish` folder
3. Set the Application Pool to **No Managed Code**
4. Add environment variables in IIS Manager → Application Pool → Advanced Settings → Environment Variables
5. Ensure the IIS app pool identity has read/write access to the publish folder

---

## Docker Deployment

See `Dockerfile` and `docker-compose.yml` in the repo root.

```bash
# Build and start with SQL Server
docker-compose up --build

# Run migrations inside the container
docker-compose exec web dotnet ef database update --project ArenaSync.Web
```

---

## Health Check

After deployment, verify the app is running:

```bash
curl -I https://your-domain.com/
# Expect: HTTP/2 200
```

Check logs for any startup errors:

```bash
# Linux systemd
journalctl -u arenasync -n 50

# Docker
docker-compose logs web --tail=50
```

---

## Rollback

To roll back a deployment:

1. Restore the previous `publish` folder
2. Roll back the last migration: `dotnet ef migrations remove --project ArenaSync.Web`
3. Apply: `dotnet ef database update --project ArenaSync.Web`
4. Restart the app
