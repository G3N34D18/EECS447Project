# ArenaSync — Development Guide

## Coding Standards

### Naming Conventions

| Element | Convention | Example |
|---|---|---|
| Classes, interfaces | PascalCase | `EventService`, `IEventService` |
| Methods | PascalCase | `GetEventByIdAsync` |
| Private fields | _camelCase | `_context`, `_logger` |
| Local variables | camelCase | `eventEntity` |
| Razor components | PascalCase | `EventEdit.razor` |
| CSS classes | kebab-case | `arena-card`, `arena-primary-btn` |

### Async/Await

- All database operations must be `async` and use `Await`.
- Never use `.Result` or `.Wait()` — these deadlock in Blazor Server.
- Method names end in `Async` when they return a `Task`.

```csharp
// Good
public async Task<Event?> GetEventByIdAsync(int id)
    => await _context.Events.FindAsync(id);

// Bad — blocks the thread
public Event? GetEventById(int id)
    => _context.Events.Find(id);
```

### Error Handling

- Services should let exceptions propagate unless they can recover meaningfully.
- Razor pages should catch exceptions in handler methods and display a user-friendly message.
- Always log exceptions with context: `_logger.LogError(ex, "Message with {Context}", value)`.

### Validation

- Put DataAnnotations attributes (`[Required]`, `[StringLength]`, etc.) on DTO classes in `Dtos/`, not on EF models.
- For cross-field or business-rule validation, use `ValidationService`.
- Always show errors via `<ValidationSummaryPanel>` — never raw `<ValidationSummary>`.

---

## Adding a New Feature

### 1. Add the model (if new entity)

Create `Models/MyEntity.cs` with DataAnnotations. Add a `DbSet<MyEntity>` to `ApplicationDbContext`.

### 2. Add a migration

```bash
dotnet ef migrations add Add_MyEntity --project ArenaSync.Web
dotnet ef database update --project ArenaSync.Web
```

### 3. Add the service

Create `Services/IMyEntityService.cs` (interface) and `Services/MyEntityService.cs` (implementation). Register in `Program.cs`:

```csharp
builder.Services.AddScoped<IMyEntityService, MyEntityService>();
```

### 4. Add pages

Create Razor pages under `Pages/MyEntity/`. Use `[SupplyParameterFromForm]` on the form model property for SSR edit/create pages.

### 5. Add tests

Add unit tests in `ArenaSync.Web.Tests/Services/MyEntityServiceTests.cs` using the `SqliteTestDatabase` helper.

---

## Running Database Migrations

```bash
# Add a new migration
dotnet ef migrations add <MigrationName> --project ArenaSync.Web

# Apply pending migrations
dotnet ef database update --project ArenaSync.Web

# Roll back the last migration
dotnet ef migrations remove --project ArenaSync.Web

# Drop and recreate from scratch
dotnet ef database drop --project ArenaSync.Web
dotnet ef database update --project ArenaSync.Web
```

---

## Running Tests

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run a specific test class
dotnet test --filter "FullyQualifiedName~ValidationServiceTests"
```

Tests use `SqliteTestDatabase` which creates an in-memory SQLite database per test class. No SQL Server needed.

---

## Logging

Use structured logging with named parameters:

```csharp
_logger.LogInformation("User {Email} logged in.", email);
_logger.LogWarning("Skipping seed for {Table} — already has rows.", "Events");
_logger.LogError(ex, "Failed to delete event {EventId}.", id);
```

Log levels by environment:

| Level | Development | Production |
|---|---|---|
| Debug | ✅ | ❌ |
| Information | ✅ | ✅ |
| Warning | ✅ | ✅ |
| Error | ✅ | ✅ |

---

## SSR Form Binding Pattern

All edit and create pages use Blazor SSR. The pattern is:

```csharp
[SupplyParameterFromForm]
private MyFormModel form { get; set; } = new();

protected override async Task OnInitializedAsync()
{
    // Only load from DB on GET, not after POST
    var isUninitialized = string.IsNullOrWhiteSpace(form.Name);
    if (isUninitialized)
    {
        var entity = await _service.GetByIdAsync(Id);
        if (entity is not null)
            PopulateForm(form, entity);
    }
}
```

Always use `<InputSelect>` and `<InputText>` (Blazor components), never plain `<select>` or `<input>`, so that `[SupplyParameterFromForm]` correctly maps POST body fields to the model.
