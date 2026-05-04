# ArenaSync — Architecture

## Folder Structure

```
EECS447Project/
├── ArenaSync.Web/                  # Main Blazor Server application
│   ├── Components/
│   │   ├── Layout/                 # MainLayout, NavMenu, ReconnectModal
│   │   └── Shared/                 # Reusable components (ValidationSummaryPanel)
│   ├── Data/
│   │   ├── ApplicationDbContext.cs # EF Core DbContext (extends IdentityDbContext)
│   │   ├── Migrations/             # EF Core migration files
│   │   └── Seed/DbSeeder.cs        # Dev-only database seeder
│   ├── Dtos/                       # Form models and report DTOs
│   ├── Helpers/                    # EventFormHelpers (date/time parsing)
│   ├── Middleware/
│   │   └── ExceptionHandlingMiddleware.cs
│   ├── Models/                     # EF Core entity models
│   ├── Pages/                      # Razor page components
│   │   ├── Auth/                   # Login, Register, Logout, AccessDenied
│   │   ├── Attendees/
│   │   ├── Events/
│   │   ├── Reports/
│   │   ├── Teams/
│   │   ├── Vendors/
│   │   └── Venues/
│   ├── Services/                   # Business logic interfaces + implementations
│   ├── Program.cs                  # App entry point / DI registration
│   └── wwwroot/                    # Static assets (CSS, images)
└── ArenaSync.Web.Tests/            # xUnit test project
    ├── Integration/                # Auth + validation integration tests
    ├── Services/                   # Unit tests per service
    └── TestSupport/                # SqliteTestDatabase, TestData helpers
```

---

## Data Model

```
Venue (1) ──< Event (many)
Venue (1) ──< LockerRoom (many)
Venue (1) ──< VendorBooth (many)

Event (1) ──< TeamAssignment (many) >── Team
TeamAssignment >── LockerRoom

Event (1) ──< VendorAssignment (many) >── Vendor
VendorAssignment >── VendorBooth

Event ──< ParticipatesIn >── Team        (many-to-many)
Event ──< RegistersFor   >── Attendee    (many-to-many)
```

All primary keys are SQL Server IDENTITY columns (auto-generated). Foreign keys are enforced at the database level.

---

## Key Design Patterns

### Service Layer
All business logic lives in `Services/`. Each domain area has an interface (`IEventService`) and a concrete implementation (`EventService`). Services are registered as `Scoped` in `Program.cs` and injected into Razor pages.

### Repository via DbContext
There is no explicit repository layer — services use `ApplicationDbContext` directly via EF Core. This keeps the codebase lean for an MVP.

### SSR Form Handling
All edit/create pages use Blazor Static Server Rendering (SSR). Forms post via standard HTTP POST. Models decorated with `[SupplyParameterFromForm]` receive the POST body automatically. `OnInitializedAsync` checks whether the form is already populated before loading from the database, preventing overwrite of submitted values.

### Validation
Two layers:
1. **DataAnnotations** (`[Required]`, `[StringLength]`, `[Range]`, `[EmailAddress]`) on DTOs — caught by `<DataAnnotationsValidator>` in the form.
2. **Business rules** in `ValidationService` — checked in service methods and surfaced via `ValidationSummaryPanel`.

### Authentication & Authorization
ASP.NET Core Identity provides user management, password hashing, and cookie-based sessions. Three roles: `Admin`, `Manager`, `Viewer`. Pages use `[Authorize]` and `[Authorize(Roles = "Admin")]` attributes. The layout uses `<AuthorizeView>` to conditionally show the sign-out link.

### Exception Handling
`ExceptionHandlingMiddleware` wraps every request. It logs the full exception, returns JSON for API requests, and redirects browsers to `/Error`.

---

## Authentication Flow

```
User submits login form (POST /auth/login)
  → SignInManager.PasswordSignInAsync()
  → On success: auth cookie set, redirect to /
  → On failure: error message shown, lockout after 5 attempts
  → On lockout: lockout message shown

Protected page requested
  → Auth middleware reads cookie
  → If unauthenticated: redirect to /auth/login?returnUrl=...
  → If wrong role: redirect to /auth/access-denied
```

---

## Technology Decisions

| Decision | Rationale |
|---|---|
| Blazor Server SSR | Simple deployment, no separate API, full server-side rendering |
| SQL Server IDENTITY PKs | Let the DB manage keys; avoids IDENTITY_INSERT issues |
| xUnit + SQLite in-memory | Fast tests with no external dependencies |
| ASP.NET Core Identity | Production-grade auth with minimal boilerplate |
| Scoped DbContext | EF Core best practice for web apps; one context per request |
