# Changelog

All notable changes to ArenaSync are documented here.

---

## [Batch 6] — 2026-05-03 — Polish, Documentation & Deployment

### Added
- Full project documentation: README, ARCHITECTURE, DEVELOPMENT_GUIDE, USER_GUIDE, DEPLOYMENT, CHANGELOG, CONTRIBUTING
- E2E manual test scripts: TESTING.md, ArenaSync.Web.Tests/E2E/E2EScenarios.md
- GitHub Actions CI/CD workflows: build-and-test.yml, deploy.yml
- GitHub issue templates (bug report, feature request) and PR template
- Dockerfile and docker-compose.yml for containerized deployment
- appsettings.Production.json with safe production defaults
- .editorconfig for consistent code style enforcement
- .env.example documenting required environment variables

### Changed
- Minor CSS polish: improved spacing, hover states, and typography consistency

---

## [Batch 5] — 2026-05-03 — Bug Fixes & Stabilization

### Fixed
- Event delete now cascades: removes ParticipatesIn, RegistersFor, TeamAssignment, and VendorAssignment rows before deleting the event
- Event edit now saves correctly: replaced plain `<select>` with `<InputSelect>` for AM/PM fields so SSR form binding works
- Login form now works: added `[SupplyParameterFromForm]` to login model so POST body is properly bound in SSR mode
- DbSeeder locker room and vendor booth FK violations: venue IDs are now looked up dynamically instead of hardcoded
- All seeder methods use SQL Server IDENTITY-generated PKs (no more explicit Id values)

---

## [Batch 4] — 2026-05-03 — Auth, Validation & Seeding

### Added
- ASP.NET Core Identity integration (login, register, logout, access denied pages)
- Role-based authorization: Admin, Manager, Viewer roles
- Global exception handling middleware (ExceptionHandlingMiddleware)
- IValidationService and ValidationService with 4 business rule validators
- ValidationSummaryPanel shared component
- DbSeeder with full demo dataset: 3 venues, 5 events, 10 teams, 20 attendees, 5 vendors, locker rooms, booths, assignments
- 73 unit and integration tests (xUnit + SQLite in-memory)
- DataAnnotations validation on all models and DTOs

### Changed
- ApplicationDbContext now extends IdentityDbContext
- Program.cs fully rewritten with Identity, cookie auth, and middleware pipeline

---

## [Batch 3] — Reporting & Advanced Features

### Added
- Reporting service with event summary, team schedule, venue occupancy, and conflict reports
- Export report page
- Assignment conflict detection
- Team reassignment request workflow

---

## [Batch 2] — Core CRUD

### Added
- Full CRUD for Events, Teams, Attendees, Venues, Vendors
- Team-event assignment with locker room management
- Vendor-event assignment with booth management
- Attendee event registration
- ParticipatesIn and RegistersFor many-to-many relationships

---

## [Batch 1] — Project Foundation

### Added
- Blazor Server project setup (.NET 10)
- Entity Framework Core with SQL Server
- Core data models: Venue, Event, Team, Attendee, Vendor, LockerRoom, VendorBooth
- ApplicationDbContext with all DbSets
- Initial migration
- MainLayout and NavMenu with ArenaSync branding
