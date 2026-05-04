# ── Build stage ──────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ArenaSync.sln .
COPY ArenaSync.Web/ArenaSync.Web.csproj ArenaSync.Web/
COPY ArenaSync.Web.Tests/ArenaSync.Web.Tests.csproj ArenaSync.Web.Tests/

RUN dotnet restore

COPY . .

RUN dotnet publish ArenaSync.Web/ArenaSync.Web.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ── Runtime stage ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "ArenaSync.Web.dll"]
