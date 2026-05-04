# Changelog

All notable changes to ArenaSync are documented here, organized by batch as defined in `MVP_PLAN.md`.

---

## [Batch 6] — Polish, Docs & Deployment

**Epic:** Final polish, documentation, deployment

### Added
- Full project documentation: `README.md`, `ARCHITECTURE.md`, `DEVELOPMENT_GUIDE.md`, `USER_GUIDE.md`, `DEPLOYMENT.md`, `CONTRIBUTING.md`
- E2E manual test scripts: `TESTING.md`, `ArenaSync.Web.Tests/E2E/E2EScenarios.md`
- GitHub Actions CI/CD workflows: `build-and-test.yml`, `deploy.yml`
- GitHub issue templates (bug report, feature request) and PR template
- `Dockerfile` and `docker-compose.yml` for containerized deployment
- `appsettings.Production.json` with safe production defaults
- `.editorconfig` for consistent code style enforcement
- `.env.example` documenting required environment variables

### Changed
- CSS polish: improved spacing, hover states, typography consistency, responsive breakpoints
- NavMenu updated with Bootstrap Icons replacing Unicode placeholder symbols
- NavMenu logo and text sizes increased for improved readability
- Top navigation bar redesigned with ArenaSync branding, gold border, and styled auth buttons

---

## [Batch 5] — Admin & Quality Layers

**Epic:** Validation, error handling, authentication

### Added
- ASP.NET Core Identity integration (login, register, logout, access denied pages)
- Role-based authorization: Admin, Manager, Viewer roles
- Global exception handling middleware (`ExceptionHandlingMiddleware`)
- `IValidationService` and `ValidationService` with business rule validators
- `ValidationSummaryPanel` shared component
- Full demo dataset seeding: 3 venues, 5 events, 10 teams, 20 attendees, 5 vendors, locker rooms, booths, and assignments
- 73 unit and integration tests (xUnit + SQLite in-memory)
- `DataAnnotations` validation attributes on all models and DTOs
- `Add_Identity_Authentication` database migration

### Changed
- `ApplicationDbContext` now extends `IdentityDbContext`
- `Program.cs` updated with Identity, cookie auth, seeding, and middleware pipeline

### Fixed
- Event delete now cascades: removes `ParticipatesIn`, `RegistersFor`, `TeamAssignment`, and `VendorAssignment` rows before deleting the event
- Event edit now saves correctly: replaced plain `<select>` with `<InputSelect>` for AM/PM fields so SSR form binding works
- Login form now works: added `[SupplyParameterFromForm]` to login model so POST body is properly bound in SSR mode
- DbSeeder locker room and vendor booth FK violations: venue IDs are now looked up dynamically instead of hardcoded
- All seeder methods use SQL Server IDENTITY-generated PKs (no explicit Id values)

---

## [Batch 4] — Dashboards & Reporting

**Epic:** Management dashboards and reporting

### Added
- `ReportingService` with dashboard summary, event roster, team schedule, venue occupancy, and conflict report methods
- Dashboard page with key metrics (events, teams, vendors, attendees, pending assignments, critical alerts)
- Detailed report pages: `EventReport`, `TeamReport`, `VenueReport`, `ConflictReport`, `ExportReport`
- Reusable shared components: `SummaryPanel`, `EntityTable`, `ReportHeader`, `ConflictWarningPanel`
- DTOs: `DashboardSummaryDto`, `EventRosterDto`, `ConflictReportRowDto`

---

## [Batch 3] — Vendor & Facility Management

**Epic:** Vendor and facility CRUD, assignment workflows, conflict validation

### Added
- Full CRUD for Vendors, LockerRooms, and VendorBooths
- `AssignmentService` with team-to-locker and vendor-to-booth assignment logic and conflict detection
- `VendorService` with full CRUD methods
- Assignment conflict detection and warning UI
- Unique constraint enforcement in database and UI
- Unit and integration tests for assignment and vendor service logic
- DTOs: `VendorFormModel`, `VendorAssignmentFormModel`, `AssignmentConflictDto`

---

## [Batch 2] — Teams & Attendee Integration

**Epic:** Team and attendee CRUD, assignments, and validation

### Added
- Full CRUD for Teams and Attendees
- Team-to-Event assignment with locker room management
- Attendee event registration
- `ParticipatesIn` and `RegistersFor` many-to-many relationship pages and logic
- Duplicate prevention for assignments and registrations
- Unit and integration tests for team and attendee services
- DTOs: `TeamFormModel`, `AttendeeFormModel`, `TeamAssignmentFormModel`

---

## [Batch 1] — Event & Venue Management Core

**Epic:** Full CRUD for Events and Venues

### Added
- Blazor Server project setup (.NET 10)
- Entity Framework Core with SQL Server
- Core data models: `Venue`, `Event`, `Team`, `Attendee`, `Vendor`, `LockerRoom`, `VendorBooth`
- `ApplicationDbContext` with all DbSets and fluent API configurations
- Initial database migration
- Full CRUD pages for Events and Venues
- `EventService` and `VenueService` with all CRUD methods
- `MainLayout` and `NavMenu` with ArenaSync branding
- Unit tests for `EventService` and `VenueService`
