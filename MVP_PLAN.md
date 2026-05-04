# ArenaSync MVP Implementation Plan - Detailed File/Folder Structure

---

**Document created 2026-04-14**  
**Repository:** G3N34D18/EECS447Project  
**Language Composition:** C# (68%), CSS (18.9%), HTML (9.2%), JavaScript (3.9%)

## Overview
This is the batch-based MVP implementation plan for ArenaSync.Web with detailed file and folder modifications/creations per batch.

---

## Batch 1: Event & Venue Management Core

**Epic:** Full CRUD for Events and Venues  
**Timeline:** Week 1-2  
**Priority:** Highest  
**Dependencies:** None  

### User Stories
- US1.1: As an admin, I want to create, read, update, and delete events so that I can manage event schedules
- US1.2: As an admin, I want to create, read, update, and delete venues so that I can manage event locations
- US1.3: As an admin, I want to navigate between events and venues so that workflow is seamless

### Tasks & File Changes

#### Models (C#)
| File | Status | Changes |
|------|--------|---------|
| `ArenaSync.Web/Models/Event.cs` | **UPDATE** | Ensure all properties are finalized: `Id`, `Name`, `StartTime`, `EndTime`, `Description`, `VenueId`, `Venue` navigation, `Assignments`, `VendorAssignments`. Add validation attributes (Required, StringLength, etc.). |
| `ArenaSync.Web/Models/Venue.cs` | **UPDATE** | Ensure all properties finalized: `Id`, `Name`, `Address`, `Capacity`. Add validation attributes. Ensure navigation properties for `Events`, `LockerRooms`, `VendorBooths`. |

#### Data Layer (C#)
| File | Status | Changes |
|------|--------|---------|
| `ArenaSync.Web/Data/ApplicationDbContext.cs` | **VERIFY** | Confirm `DbSet<Event>`, `DbSet<Venue>` are present. Verify composite key and foreign key configurations for Events/Venues. |
| `ArenaSync.Web/Data/Migrations/` | **CREATE** | Generate migration: `Add_Event_Venue_Initial` with `dotnet ef migrations add` if not already done. |
| `ArenaSync.Web/Data/Configurations/EventConfiguration.cs` | **CREATE/COMPLETE** | Implement IEntityTypeConfiguration<Event> with fluent API configuration (if empty scaffold exists). |
| `ArenaSync.Web/Data/Configurations/VenueConfiguration.cs` | **CREATE/COMPLETE** | Implement IEntityTypeConfiguration<Venue> with fluent API configuration. |
| `ArenaSync.Web/Data/Seed/DbSeeder.cs` | **UPDATE** | Add seed method for 3-5 sample venues and 5-10 sample events. |

#### Services (C#)
| File | Status | Changes |
|------|--------|---------|
| `ArenaSync.Web/Services/EventService.cs` | **IMPLEMENT** | Add methods: `GetAllEventsAsync()`, `GetEventByIdAsync(int id)`, `CreateEventAsync(Event event)`, `UpdateEventAsync(Event event)`, `DeleteEventAsync(int id)`. Use `IApplicationDbContext` or direct context injection. |
| `ArenaSync.Web/Services/VenueService.cs` | **IMPLEMENT** | Add methods: `GetAllVenuesAsync()`, `GetVenueByIdAsync(int id)`, `CreateVenueAsync(Venue venue)`, `UpdateVenueAsync(Venue venue)`, `DeleteVenueAsync(int id)`. Include check for associated events before delete. |
| `ArenaSync.Web/Program.cs` | **UPDATE** | Register services: `builder.Services.AddScoped<IEventService, EventService>();` and `builder.Services.AddScoped<IVenueService, VenueService>();` (or use implicit interface if not created). |

#### Pages/Components (Razor)
| File | Status | Changes |
|------|--------|---------|
| `ArenaSync.Web/Pages/Events/Index.razor` | **CREATE** | Create event list page with table showing Id, Name, StartTime, VenueName, Actions (Edit/Delete). Add pagination. Add "Create New" button. |
| `ArenaSync.Web/Pages/Events/Create.razor` | **CREATE** | Create form for new events with fields: Name, StartTime, EndTime, Description, VenueId (dropdown). Include validation feedback. Submit calls EventService.CreateEventAsync(). |
| `ArenaSync.Web/Pages/Events/Edit.razor` | **CREATE** | Edit form similar to Create but loads existing event by Id. Update calls EventService.UpdateEventAsync(). |
| `ArenaSync.Web/Pages/Events/Delete.razor` | **CREATE** | Confirmation modal/page for event deletion. Delete calls EventService.DeleteEventAsync(). |
| `ArenaSync.Web/Pages/Venues/Index.razor` | **CREATE** | Create venue list page with table showing Id, Name, Address, Capacity, Actions. Add "Create New" button. |
| `ArenaSync.Web/Pages/Venues/Create.razor` | **CREATE** | Form for new venues with fields: Name, Address, Capacity. Submit calls VenueService.CreateVenueAsync(). |
| `ArenaSync.Web/Pages/Venues/Edit.razor` | **CREATE** | Edit form similar to Create but for existing venues. |
| `ArenaSync.Web/Pages/Venues/Delete.razor` | **CREATE** | Confirmation for venue deletion (with check for associated events). |

#### DTOs (C#)
| File | Status | Changes |
|------|--------|---------|
| `ArenaSync.Web/Dtos/EventFormModel.cs` | **IMPLEMENT** | Create DTO with `Name`, `StartTime`, `EndTime`, `Description`, `VenueId`. Add validation attributes (Required, StringLength, Range for dates). |
| `ArenaSync.Web/Dtos/VenueFormModel.cs` | **CREATE** | Create DTO with `Name`, `Address`, `Capacity`. Add validation attributes. |

#### Navigation (Razor)
| File | Status | Changes |
|------|--------|---------|
| `ArenaSync.Web/Components/Layout/NavMenu.razor` | **UPDATE** | Replace placeholder "Counter" and "Weather" links with: - `<NavLink href="events" class="nav-link">Events</NavLink>` - `<NavLink href="venues" class="nav-link">Venues</NavLink>` |
| `ArenaSync.Web/Components/Routes.razor` | **UPDATE** | Add route definitions: `<Route Path="events" Component={typeof(EventIndex)} />`, `<Route Path="events/create" Component={typeof(EventCreate)} />`, etc. for all CRUD pages. |

#### Tests (C#)
| File | Status | Changes |
|------|--------|---------|
| `ArenaSync.Web.Tests/Services/EventServiceTests.cs` | **CREATE** | Unit tests for EventService: test GetAllEventsAsync, GetEventByIdAsync, CreateEventAsync (valid/invalid), UpdateEventAsync, DeleteEventAsync. Use mocking for DbContext. Aim for 80%+ coverage. |
| `ArenaSync.Web.Tests/Services/VenueServiceTests.cs` | **CREATE** | Unit tests for VenueService with same pattern. Include test for preventing deletion when events exist. |

#### Configuration
| File | Status | Changes |
|------|--------|---------|
| `ArenaSync.Web/appsettings.json` | **VERIFY** | Ensure connection string points to `ArenaSyncDb`. |
| `ArenaSync.Web/appsettings.Development.json` | **VERIFY** | Confirm development overrides if needed. |

### Deliverables
- ✅ Events and Venues fully CRUD-able via UI
- ✅ Navigation menu updated
- ✅ Service layer complete with unit tests (80%+ coverage)
- ✅ Database migrations applied
- ✅ All validation working

---

## Batch 2: Teams & Attendee Integration

**Epic:** Team and attendee CRUD, assignments, and validation  
**Timeline:** Week 3-4  
**Priority:** Highest  
**Dependencies:** Batch 1 complete  

### User Stories
- US2.1: As an admin, I want to manage teams (CRUD) so that I can organize participants
- US2.2: As an admin, I want to manage attendees (CRUD) so that I can track individual participants
- US2.3: As an admin, I want to assign teams to events so that teams can participate
- US2.4: As an admin, I want to register attendees for events so that attendance is tracked

### Tasks & File Changes

#### Models (C#)
| File | Status | Changes |
|------|--------|---------|
| `ArenaSync.Web/Models/Team.cs` | **UPDATE** | Finalize: `Id`, `Name`, `Manager`, `Email`, `Phone`, `PlayerCount`. Add validation. Ensure `ParticipatesIn` and `Assignments` navigation properties. |
| `ArenaSync.Web/Models/Attendee.cs` | **UPDATE** | Finalize: `Id`, `Name`, `Email`, `Phone`. Add validation. Ensure navigation property for `RegistersFor`. |
| `ArenaSync.Web/Models/ParticipatesIn.cs` | **VERIFY** | Confirm composite key `(TeamId, EventId)`. Ensure navigation properties `Team` and `Event`. |
| `ArenaSync.Web/Models/RegistersFor.cs` | **VERIFY** | Confirm composite key `(AttendeeId, EventId)`. Ensure navigation properties `Attendee` and `Event`. |

#### Data Layer (C#)
| File | Status | Changes |
|------|--------|---------|
| `ArenaSync.Web/Data/ApplicationDbContext.cs` | **VERIFY** | Confirm `DbSet<Team>`, `DbSet<Attendee>`, `DbSet<ParticipatesIn>`, `DbSet<RegistersFor>` present. Verify composite keys and constraints in OnModelCreating. |
| `ArenaSync.Web/Data/Migrations/` | **CREATE** | Generate migration: `Add_Team_Attendee_ParticipatesIn_RegistersFor` if not done. Apply migration. |
| `ArenaSync.Web/Data/Configurations/TeamConfiguration.cs` | **CREATE/COMPLETE** | Implement fluent API configuration for Team. |
| `ArenaSync.Web/Data/Configurations/AttendeeConfiguration.cs` | **CREATE/COMPLETE** | Implement fluent API configuration for Attendee. |
| `ArenaSync.Web/Data/Configurations/ParticipatesInConfiguration.cs` | **CREATE/COMPLETE** | Configure composite key and relationships. |
| `ArenaSync.Web/Data/Configurations/RegistersForConfiguration.cs` | **CREATE/COMPLETE** | Configure composite key and relationships. |
| `ArenaSync.Web/Data/Seed/DbSeeder.cs` | **UPDATE** | Add seed methods for 10 teams and 20 attendees. Add some sample ParticipatesIn and RegistersFor records. |

#### Services (C#)
| File | Status | Changes |
|------|--------|---------|
| `ArenaSync.Web/Services/TeamService.cs` | **IMPLEMENT** | Add methods: `GetAllTeamsAsync()`, `GetTeamByIdAsync(int id)`, `CreateTeamAsync(Team team)`, `UpdateTeamAsync(Team team)`, `DeleteTeamAsync(int id)`, `AssignTeamToEventAsync(int teamId, int eventId)`, `GetTeamsForEventAsync(int eventId)`. Add duplicate check logic. |
| `ArenaSync.Web/Services/AttendeeService.cs` | **IMPLEMENT** | Add methods: `GetAllAttendeesAsync()`, `GetAttendeeByIdAsync(int id)`, `CreateAttendeeAsync(Attendee attendee)`, `UpdateAttendeeAsync(Attendee attendee)`, `DeleteAttendeeAsync(int id)`, `RegisterAttendeeForEventAsync(int attendeeId, int eventId)`, `GetAttendeesForEventAsync(int eventId)`. Add duplicate check. |
| `ArenaSync.Web/Program.cs` | **UPDATE** | Register `TeamService` and `AttendeeService`. |

#### Pages/Components (Razor)
| File | Status | Changes |
|------|--------|---------|
| `ArenaSync.Web/Pages/Teams/Index.razor` | **CREATE** | Team list with table (Id, Name, Manager, Email, PlayerCount, Actions). Add "Create New" button. |
| `ArenaSync.Web/Pages/Teams/Create.razor` | **CREATE** | Form for new teams. Fields: Name, Manager, Email, Phone, PlayerCount. |
| `ArenaSync.Web/Pages/Teams/Edit.razor` | **CREATE** | Edit form for existing teams. |
| `ArenaSync.Web/Pages/Teams/Delete.razor` | **CREATE** | Confirmation page. |
| `ArenaSync.Web/Pages/Teams/AssignToEvent.razor` | **CREATE** | Modal or page to assign team to event. Dropdown for event selection. Check for duplicates. |
| `ArenaSync.Web/Pages/Attendees/Index.razor` | **CREATE** | Attendee list with table (Id, Name, Email, Phone, Actions). |
| `ArenaSync.Web/Pages/Attendees/Create.razor` | **CREATE** | Form for new attendees. Fields: Name, Email, Phone. |
| `ArenaSync.Web/Pages/Attendees/Edit.razor` | **CREATE** | Edit form. |
| `ArenaSync.Web/Pages/Attendees/Delete.razor` | **CREATE** | Confirmation page. |
| `ArenaSync.Web/Pages/Attendees/RegisterForEvent.razor` | **CREATE** | Modal or page to register attendee for event. Dropdown for event selection. Check for duplicates. |

#### DTOs (C#)
| File | Status | Changes |
|------|--------|---------|
| `ArenaSync.Web/Dtos/TeamFormModel.cs` | **IMPLEMENT** | DTO with `Name`, `Manager`, `Email`, `Phone`, `PlayerCount`. Add validation. |
| `ArenaSync.Web/Dtos/AttendeeFormModel.cs` | **CREATE** | DTO with `Name`, `Email`, `Phone`. Add validation. |
| `ArenaSync.Web/Dtos/TeamAssignmentFormModel.cs` | **IMPLEMENT** | DTO for assignment with `TeamId`, `EventId`. Add validation. |

#### Navigation (Razor)
| File | Status | Changes |
|------|--------|---------|
| `ArenaSync.Web/Components/Layout/NavMenu.razor` | **UPDATE** | Add links: - `<NavLink href="teams" class="nav-link">Teams</NavLink>` - `<NavLink href="attendees" class="nav-link">Attendees</NavLink>` |
| `ArenaSync.Web/Components/Routes.razor` | **UPDATE** | Add routes for all Team and Attendee pages (Index, Create, Edit, Delete, AssignToEvent, RegisterForEvent). |

#### Tests (C#)
| File | Status | Changes |
|------|--------|---------|
| `ArenaSync.Web.Tests/Services/TeamServiceTests.cs` | **CREATE** | Unit tests for TeamService CRUD and assignment logic. Test duplicate prevention. 80%+ coverage. |
| `ArenaSync.Web.Tests/Services/AttendeeServiceTests.cs` | **CREATE** | Unit tests for AttendeeService CRUD and registration logic. 80%+ coverage. |
| `ArenaSync.Web.Tests/Integration/TeamEventAssignmentTests.cs` | **CREATE** | Integration tests for team-event assignment workflows. |

### Deliverables
- ✅ Teams and Attendees fully CRUD-able
- ✅ Team-to-Event and Attendee-to-Event assignment UI and logic
- ✅ Duplicate prevention enforced
- ✅ Navigation updated
- ✅ Full test coverage

---

## Batch 3: Vendor & Facility Management

**Epic:** Vendor and facility CRUD, assignment workflows, conflict validation  
**Timeline:** Week 5-6  
**Priority:** Highest  
**Dependencies:** Batch 1 complete  

### User Stories
- US3.1: As an admin, I want to manage vendors (CRUD) so that I can organize vendors
- US3.2: As an admin, I want to manage locker rooms and vendor booths so that I can allocate physical spaces
- US3.3: As an admin, I want to assign vendors to booths and teams to lockers so that space is optimized
- US3.4: As an admin, I want to see assignment conflicts and warnings so that I can resolve issues

### Tasks & File Changes

#### Models (C#)
| File | Status | Changes |
|------|--------|---------|
| `ArenaSync.Web/Models/Vendor.cs` | **UPDATE** | Finalize: `Id`, `Name`, `Type`, `Location`, `Phone`. Add validation. Ensure `SuppliesAt` and `Assignments` navigation. |
| `ArenaSync.Web/Models/LockerRoom.cs` | **UPDATE** | Finalize: `Id`, `RoomNumber`, `VenueId`, `Venue` navigation, `Assignments`. |
| `ArenaSync.Web/Models/VendorBooth.cs` | **UPDATE** | Finalize: `Id`, `BoothNumber`, `VenueId`, `Venue` navigation, `VendorAssignments`. |
| `ArenaSync.Web/Models/SuppliesAt.cs` | **VERIFY** | Composite key `(VendorId, EventId)`. Navigation: `Vendor`, `Event`. |
| `ArenaSync.Web/Models/TeamAssignment.cs` | **VERIFY** | Composite key `(TeamId, EventId, LockerId)`. Navigation: `Team`, `Event`, `LockerRoom`. |
| `ArenaSync.Web/Models/VendorAssignment.cs` | **VERIFY** | Composite key `(VendorId, EventId, BoothId)`. Navigation: `Vendor`, `Event`, `Booth`. Unique constraints on (EventId, VendorId) and (EventId, BoothId). |

#### Data Layer (C#)
| File | Status | Changes |
|------|--------|---------|
| `ArenaSync.Web/Data/ApplicationDbContext.cs` | **VERIFY** | Confirm all DbSets present. Verify all composite keys and unique constraints in OnModelCreating. |
| `ArenaSync.Web/Data/Migrations/` | **CREATE** | Generate migration: `Add_Vendor_Facility_Assignments` if not done. |
| `ArenaSync.Web/Data/Configurations/VendorConfiguration.cs` | **CREATE/COMPLETE** | Fluent API configuration. |
| `ArenaSync.Web/Data/Configurations/LockerRoomConfiguration.cs` | **CREATE/COMPLETE** | Configure relationship to Venue. |
| `ArenaSync.Web/Data/Configurations/VendorBoothConfiguration.cs` | **CREATE/COMPLETE** | Configure relationship to Venue. |
| `ArenaSync.Web/Data/Configurations/SuppliesAtConfiguration.cs` | **CREATE/COMPLETE** | Configure composite key and relationships. |
| `ArenaSync.Web/Data/Configurations/TeamAssignmentConfiguration.cs` | **CREATE/COMPLETE** | Configure composite key, relationships, and unique constraints. |
| `ArenaSync.Web/Data/Configurations/VendorAssignmentConfiguration.cs` | **CREATE/COMPLETE** | Configure composite key, relationships, and unique constraints. |
| `ArenaSync.Web/Data/Seed/DbSeeder.cs` | **UPDATE** | Add seed for 3 vendors, 5-10 locker rooms, 5-10 vendor booths, some sample assignments. |

#### Services (C#)
| File | Status | Changes |
|------|--------|---------|
| `ArenaSync.Web/Services/VendorService.cs` | **IMPLEMENT** | Methods: `GetAllVendorsAsync()`, `GetVendorByIdAsync(int id)`, `CreateVendorAsync(Vendor vendor)`, `UpdateVendorAsync(Vendor vendor)`, `DeleteVendorAsync(int id)`, `GetVendorsForEventAsync(int eventId)`. |
| `ArenaSync.Web/Services/AssignmentService.cs` | **IMPLEMENT** | Methods: `AssignTeamToLockerAsync(int teamId, int eventId, int lockerId)` with uniqueness checks, `AssignVendorToBoothAsync(int vendorId, int eventId, int boothId)` with uniqueness checks, `GetAssignmentConflictsAsync(int eventId)` (returns list of conflicts), `CheckTeamLockerConflictAsync(int eventId, int teamId, int lockerId)`, `CheckVendorBoothConflictAsync(int eventId, int vendorId, int boothId)`. |
| `ArenaSync.Web/Program.cs` | **UPDATE** | Register `VendorService` and `AssignmentService`. |

#### Pages/Components (Razor)
| File | Status | Changes |
|------|--------|---------|
| `ArenaSync.Web/Pages/Vendors/Index.razor` | **CREATE** | Vendor list with table (Id, Name, Type, Location, Phone, Actions). |
| `ArenaSync.Web/Pages/Vendors/Create.razor` | **CREATE** | Form for new vendors. |
| `ArenaSync.Web/Pages/Vendors/Edit.razor` | **CREATE** | Edit form. |
| `ArenaSync.Web/Pages/Vendors/Delete.razor` | **CREATE** | Confirmation page. |
| `ArenaSync.Web/Pages/Venues/LockerRooms.razor` | **CREATE** | List locker rooms for a venue. CRUD forms inline or separate pages. |
| `ArenaSync.Web/Pages/Venues/VendorBooths.razor` | **CREATE** | List vendor booths for a venue. CRUD forms. |
| `ArenaSync.Web/Pages/Assignments/TeamLockerAssign.razor` | **CREATE** | Modal/page to assign team to locker room in an event. Dropdowns for team, locker, event. Validate conflicts. |
| `ArenaSync.Web/Pages/Assignments/VendorBoothAssign.razor` | **CREATE** | Modal/page to assign vendor to booth in an event. Validate conflicts. |
| `ArenaSync.Web/Pages/Assignments/ConflictView.razor` | **CREATE** | Show detected conflicts (duplicate assignments, insufficient space, etc.). Allow bulk reassignment or resolution. |
| `ArenaSync.Web/Components/Shared/ConflictWarningPanel.razor` | **CREATE** | Reusable component to display assignment conflict warnings on event/assignment pages. |

#### DTOs (C#)
| File | Status | Changes |
|------|--------|---------|
| `ArenaSync.Web/Dtos/VendorFormModel.cs` | **IMPLEMENT** | DTO with `Name`, `Type`, `Location`, `Phone`. |
| `ArenaSync.Web/Dtos/TeamAssignmentFormModel.cs` | **UPDATE** | Ensure includes `TeamId`, `EventId`, `LockerId`. Add validation. |

#### Tests (C#)
| File | Status | Changes |
|------|--------|---------|
| `ArenaSync.Web.Tests/Services/AssignmentServiceTests.cs` | **CREATE** | Unit tests for assignment logic: test uniqueness constraints, conflict detection, valid assignments. Mock DbContext. 80%+ coverage. |
| `ArenaSync.Web.Tests/Services/VendorServiceTests.cs` | **CREATE** | Unit tests for VendorService CRUD. |
| `ArenaSync.Web.Tests/Integration/AssignmentConflictTests.cs` | **CREATE** | Integration tests for conflict scenarios (e.g., try to assign same vendor to 2 booths in one event). |

### Deliverables
- ✅ Vendors, LockerRooms, VendorBooths fully CRUD-able
- ✅ Team-to-Locker and Vendor-to-Booth assignment UI and logic
- ✅ Conflict detection and warnings working
- ✅ Unique constraint enforcement in DB and UI
- ✅ Full test coverage for assignment logic

---

## Batch 4: Dashboards & Reporting

**Epic:** Management dashboards and reporting  
**Timeline:** Week 7  
**Priority:** High  
**Dependencies:** Batches 1-3 complete  

### User Stories
- US4.1: As a manager, I want to see a dashboard summary of events, teams, and assignments so that I can quickly assess system state
- US4.2: As a manager, I want detailed reports for events, teams, and venues so that I can analyze data
- US4.3: As a manager, I want to see assignment and resource conflict summaries so that I can prioritize resolution

### Tasks & File Changes

#### Services (C#)
| File | Status | Changes |
|------|--------|---------|
| `ArenaSync.Web/Services/ReportingService.cs` | **CREATE** | Methods: `GetDashboardSummaryAsync()` (returns counts: totalEvents, totalTeams, totalVendors, totalAttendees, pendingAssignments), `GetEventRosterAsync(int eventId)` (teams, attendees, assignments), `GetTeamScheduleAsync(int teamId)` (events team participates in), `GetVenueOccupancyAsync(int venueId)` (used lockers, booths, capacity), `GetConflictSummaryAsync(int eventId)` (unresolved conflicts), `ExportReportToCsvAsync(ReportType type, int? entityId)` (CSV export). |
| `ArenaSync.Web/Program.cs` | **UPDATE** | Register `ReportingService`. |

#### Pages/Components (Razor)
| File | Status | Changes |
|------|--------|---------|
| `ArenaSync.Web/Pages/Dashboard.razor` | **CREATE** | Dashboard page with summary panels showing total counts (events, teams, vendors, attendees), pending assignments count, recent events, critical alerts/conflicts. Link to detailed reports. |
| `ArenaSync.Web/Pages/Reports/EventReport.razor` | **CREATE** | Report page for a selected event. Show event details, teams participating, attendees registered, vendor booths assigned, locker room assignments. |
| `ArenaSync.Web/Pages/Reports/TeamReport.razor` | **CREATE** | Report page for a selected team. Show team info, events participating in, member count, assigned lockers. |
| `ArenaSync.Web/Pages/Reports/VenueReport.razor` | **CREATE** | Report page for a selected venue. Show capacity, number of locker rooms, vendor booths, usage per event. |
| `ArenaSync.Web/Pages/Reports/ConflictReport.razor` | **CREATE** | Report showing all unresolved assignment conflicts across all events. Allow filtering by event, status. |
| `ArenaSync.Web/Pages/Reports/ExportReport.razor` | **CREATE** | Page to select report type and export to CSV. |
| `ArenaSync.Web/Components/Shared/SummaryPanel.razor` | **CREATE** | Reusable component displaying count + label + optional drill-down link. Used on dashboard. |
| `ArenaSync.Web/Components/Shared/EntityTable.razor` | **IMPLEMENT** | General-purpose table component accepting data collection and column config. Supports sorting, pagination, filtering, row actions. |
| `ArenaSync.Web/Components/Shared/ReportHeader.razor` | **CREATE** | Reusable header for report pages showing title, filters, export button. |

#### DTOs (C#)
| File | Status | Changes |
|------|--------|---------|
| `ArenaSync.Web/Dtos/DashboardSummaryDto.cs` | **CREATE** | DTO with `TotalEvents`, `TotalTeams`, `TotalVendors`, `TotalAttendees`, `PendingAssignments`. |
| `ArenaSync.Web/Dtos/EventRosterDto.cs` | **CREATE** | DTO with event info + list of teams + list of attendees + list of assignments. |
| `ArenaSync.Web/Dtos/ConflictDto.cs` | **CREATE** | DTO with `ConflictId`, `EventId`, `Type` (enum: DuplicateTeam, DuplicateVendor, etc.), `Description`, `AffectedEntities`. |

#### Navigation (Razor)
| File | Status | Changes |
|------|--------|---------|
| `ArenaSync.Web/Components/Layout/NavMenu.razor` | **UPDATE** | Add link: - `<NavLink href="dashboard" class="nav-link">Dashboard</NavLink>` - `<NavLink href="reports" class="nav-link">Reports</NavLink>` |
| `ArenaSync.Web/Components/Routes.razor` | **UPDATE** | Add routes: `Dashboard`, `EventReport`, `TeamReport`, `VenueReport`, `ConflictReport`, `ExportReport`. |

#### Tests (C#)
| File | Status | Changes |
|------|--------|---------|
| `ArenaSync.Web.Tests/Services/ReportingServiceTests.cs` | **CREATE** | Unit tests for ReportingService. Test GetDashboardSummaryAsync, GetEventRosterAsync, conflict queries. Mock DbContext. 80%+ coverage. |
| `ArenaSync.Web.Tests/Integration/ReportingIntegrationTests.cs` | **CREATE** | Integration tests using real data to verify report accuracy. |

### Deliverables
- ✅ Dashboard page with key metrics
- ✅ Detailed reports for events, teams, venues
- ✅ Conflict summary report
- ✅ CSV export functionality
- ✅ Reusable reporting components (table, summary panel, header)
- ✅ Full test coverage

---

## Batch 5: Admin & Quality Layers

**Epic:** Validation, error handling, authentication  
**Timeline:** Week 8  
**Priority:** High  
**Dependencies:** Batches 1-4 complete  

### User Stories
- US5.1: As an admin, I want user authentication so that only authorized users access the system
- US5.2: As a system, I want robust validation so that invalid data is never persisted
- US5.3: As an end user, I want clear error messages so that I can understand what went wrong
- US5.4: As a tester, I want seeded sample data so that I can quickly QA features

### Tasks & File Changes

#### Configuration (C#)
| File | Status | Changes |
|------|--------|---------|
| `ArenaSync.Web/Program.cs` | **MAJOR UPDATE** | Add ASP.NET Core Identity setup: `builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>()`. Add authentication/authorization middleware: `app.UseAuthentication(); app.UseAuthorization();`. Register roles (Admin, Manager, Viewer) on startup. |
| `ArenaSync.Web/Data/ApplicationDbContext.cs` | **UPDATE** | Change base class to `IdentityDbContext<IdentityUser>` instead of `DbContext`. Ensure all Identity tables included. |
| `ArenaSync.Web/Data/Migrations/` | **CREATE** | Generate migration: `Add_Identity_Authentication`. Apply migration. |

#### Authentication Pages (Razor)
| File | Status | Changes |
|------|--------|---------|
| `ArenaSync.Web/Pages/Auth/Login.razor` | **CREATE** | Login page with email + password form. Post to `SignInManager.PasswordSignInAsync()`. Redirect to dashboard on success. |
| `ArenaSync.Web/Pages/Auth/Register.razor` | **CREATE** | Registration page (admin-only or public). Email + password + confirm password. Post to `UserManager.CreateAsync()`. Assign default role. |
| `ArenaSync.Web/Pages/Auth/Logout.razor` | **CREATE** | Simple logout page. Call `SignInManager.SignOutAsync()`. |
| `ArenaSync.Web/Pages/Auth/AccessDenied.razor` | **CREATE** | Page shown when user lacks authorization. |

#### Models (C#)
| File | Status | Changes |
|------|--------|---------|
| `ArenaSync.Web/Models/Event.cs` | **UPDATE** | Add validation attributes: `[Required]`, `[StringLength(200)]`, `[Range(...)]` for dates. |
| `ArenaSync.Web/Models/Venue.cs` | **UPDATE** | Add validation: `[Required]`, `[StringLength]`, `[Range(1, ...)]` for capacity. |
| `ArenaSync.Web/Models/Team.cs` | **UPDATE** | Add validation: `[Required]`, `[EmailAddress]`, `[Phone]`, `[Range(1, ...)]` for player count. |
| `ArenaSync.Web/Models/Attendee.cs` | **UPDATE** | Add validation: `[Required]`, `[EmailAddress]`, `[Phone]`. |
| `ArenaSync.Web/Models/Vendor.cs` | **UPDATE** | Add validation: `[Required]`, `[StringLength]`, `[Phone]`. |
| All DTOs in `ArenaSync.Web/Dtos/` | **UPDATE** | Add validation attributes (Required, StringLength, EmailAddress, Phone, etc.) to all form models. |

#### Components (Razor/HTML)
| File | Status | Changes |
|------|--------|---------|
| `ArenaSync.Web/Components/_Imports.razor` | **UPDATE** | Add imports for validation: `@using System.ComponentModel.DataAnnotations`. |
| `ArenaSync.Web/Components/Shared/ValidationSummaryPanel.razor` | **IMPLEMENT** | Component to display all validation errors for a form. Takes `EditContext` as parameter. Shows user-friendly error list. |
| `ArenaSync.Web/Pages/Error.razor` | **UPDATE** | Enhance with detailed error logging and user-friendly messaging. Log exception details to console/file. Display generic message to user. |
| All form pages (Create/Edit in Batch 1-3) | **UPDATE** | Wrap forms in `<EditForm>` with model binding. Add `<DataAnnotationsValidator/>` and `<ValidationSummary/>` or use custom ValidationSummaryPanel. Add [Authorize] attributes to pages. |
| `ArenaSync.Web/Components/Layout/MainLayout.razor` | **UPDATE** | Add user info display (logged-in username, logout button) in top-right. Add breadcrumb navigation. |

#### Services (C#)
| File | Status | Changes |
|------|--------|---------|
| `ArenaSync.Web/Services/` | **UPDATE ALL** | Add `[Authorize]` and `[Authorize(Roles = "Admin")]` attributes where appropriate (e.g., on delete, update). Add try-catch blocks with proper exception logging. Return `Result<T>` or `ServiceResponse<T>` wrapper with error messages for UI to display. |
| `ArenaSync.Web/Services/ValidationService.cs` | **CREATE** | Service with methods to validate complex business rules: `ValidateTeamAssignmentAsync(int teamId, int eventId)`, `ValidateVendorBoothAssignmentAsync(int vendorId, int boothId, int eventId)`, etc. Return validation errors as list of strings. |

#### Exception Handling (C#)
| File | Status | Changes |
|------|--------|---------|
| `ArenaSync.Web/Middleware/ExceptionHandlingMiddleware.cs` | **CREATE** | Custom middleware to catch unhandled exceptions, log them, and return JSON error response or redirect to error page. |
| `ArenaSync.Web/Program.cs` | **UPDATE** | Register exception handling middleware: `app.UseMiddleware<ExceptionHandlingMiddleware>();` (early in pipeline). |

#### Seeding (C#)
| File | Status | Changes |
|------|--------|---------|
| `ArenaSync.Web/Data/Seed/DbSeeder.cs` | **COMPLETE** | Implement full seeding: Create 3-5 test users (admin, manager, viewer roles), 5-10 sample events, 10 teams, 3 venues, 20 attendees, vendors, locker rooms, booths, and sample assignments. Call from `Program.cs` on startup (if development environment). |
| `ArenaSync.Web/Program.cs` | **UPDATE** | Add seeding call: `using var scope = app.Services.CreateScope(); var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>(); await seeder.SeedAsync();` |

#### Configuration Files
| File | Status | Changes |
|------|--------|---------|
| `ArenaSync.Web/appsettings.json` | **UPDATE** | Add logging configuration: `"Logging": { "LogLevel": { "Default": "Information", "Microsoft.AspNetCore": "Warning" } }`. Add section for authentication settings if needed. |
| `ArenaSync.Web/appsettings.Development.json` | **UPDATE** | Add development-specific settings: more verbose logging, seed data enabled flag. |

#### Tests (C#)
| File | Status | Changes |
|------|--------|---------|
| `ArenaSync.Web.Tests/Services/ValidationServiceTests.cs` | **CREATE** | Unit tests for all validation logic. Test happy paths and validation failures. |
| `ArenaSync.Web.Tests/Integration/AuthenticationTests.cs` | **CREATE** | Integration tests: register user, login, verify roles, access control. |
| `ArenaSync.Web.Tests/Integration/ValidationTests.cs` | **CREATE** | Integration tests: try to create invalid entities, verify error messages. |

### Deliverables
- ✅ Authentication system (Identity) fully integrated
- ✅ Role-based authorization on all pages/services
- ✅ Server + client validation on all forms
- ✅ Global exception handling with logging
- ✅ Clear error messages for all validation failures
- ✅ Database seeded with test users and data
- ✅ All validation/auth tests passing

---

## Batch 6: Polish, Docs & Deployment

**Epic:** Final polish, documentation, deployment  
**Timeline:** Week 9  
**Priority:** Medium  
**Dependencies:** Batches 1-5 complete  

### User Stories
- US6.1: As a developer, I want comprehensive documentation so that I can onboard quickly
- US6.2: As a user, I want polished UI so that the system feels professional
- US6.3: As an operations team, I want deployed MVP so that stakeholders can demo
- US6.4: As a QA tester, I want end-to-end test scripts so that I can validate workflows

### Tasks & File Changes

#### UI & Styling (CSS/HTML)
| File | Status | Changes |
|------|--------|---------|
| `ArenaSync.Web/wwwroot/css/app.css` | **UPDATE** | Enhance global styles: consistent color scheme, typography, spacing. Add responsive breakpoints for mobile/tablet. |
| `ArenaSync.Web/Components/Layout/MainLayout.razor.css` | **ENHANCE** | Improve sidebar styling, main content area, responsive layout. Add smooth transitions. |
| `ArenaSync.Web/Components/Layout/NavMenu.razor.css` | **ENHANCE** | Polish navigation menu: better hover states, active link highlighting, responsive mobile menu. |
| All page `.razor.css` files | **CREATE/UPDATE** | Add component-specific styling for consistency. Ensure responsive design. Use Bootstrap utilities where applicable. |
| `ArenaSync.Web/wwwroot/` | **ADD** | Add any images, icons, or additional assets needed for professional look. |

#### Layout & Navigation
| File | Status | Changes |
|------|--------|---------|
| `ArenaSync.Web/Components/Layout/MainLayout.razor` | **UPDATE** | Add breadcrumb navigation component. Display current user info. Add footer with version/copyright. |
| `ArenaSync.Web/Components/Layout/NavMenu.razor` | **UPDATE** | Organize menu by sections (Events, Teams, Attendees, Vendors, Reports, Admin). Add responsive collapsible menu for mobile. |
| `ArenaSync.Web/Components/Layout/Breadcrumb.razor` | **CREATE** | Reusable breadcrumb component showing current page hierarchy. |
| `ArenaSync.Web/Components/Layout/Footer.razor` | **CREATE** | Footer with copyright, version, links. |

#### Documentation (Markdown)
| File | Status | Changes |
|------|--------|---------|
| `README.md` | **CREATE** | File in repo root. Include: project overview, tech stack, prerequisites, setup instructions (clone, build, run, test commands), connection string setup, seeding instructions, demo user credentials, troubleshooting. |
| `ARCHITECTURE.md` | **CREATE** | File in repo root. Explain folder structure, key design patterns, data model relationships, service layer architecture, authentication flow. Include diagrams if possible. |
| `DEVELOPMENT_GUIDE.md` | **CREATE** | File in repo root. Cover: coding standards (naming conventions, async/await patterns), how to add new features, running tests, database migrations workflow, logging best practices. |
| `USER_GUIDE.md` | **CREATE** | File in repo root. Step-by-step walkthroughs for each major feature: creating events, assigning teams, managing vendors, viewing reports. Include screenshots. |
| `DEPLOYMENT.md` | **CREATE** | File in repo root. Deployment steps, environment configuration, production connection string setup, database migration on production. |
| `CHANGELOG.md` | **CREATE** | File in repo root. Track MVP milestones (Batch 1-6 completion dates). |
| `CONTRIBUTING.md` | **CREATE** | File in repo root. Guidelines for contributing code, PR process, code review checklist. |
| `.github/ISSUE_TEMPLATE/` | **CREATE** | Issue templates for bug reports, feature requests. |
| `.github/PULL_REQUEST_TEMPLATE.md` | **CREATE** | PR template with checklist (tests passing, docs updated, etc.). |

#### Testing Scripts
| File | Status | Changes |
|------|--------|---------|
| `TESTING.md` | **CREATE** | File in repo root. E2E test scenarios as step-by-step instructions: "Test Event Creation", "Test Team Assignment", "Test Report Generation", etc. Include expected results. |
| `ArenaSync.Web.Tests/E2E/E2EScenarios.md` | **CREATE** | Detailed E2E test scripts in Markdown or PDF format for manual QA. |

#### Build & Deployment Config
| File | Status | Changes |
|------|--------|---------|
| `ArenaSync.Web/appsettings.Production.json` | **CREATE** | Production configuration: connection string (env var placeholder), logging level, security headers, etc. |
| `.github/workflows/build-and-test.yml` | **CREATE** | CI/CD pipeline (GitHub Actions): build, run tests, report coverage. Trigger on push/PR. |
| `.github/workflows/deploy.yml` | **CREATE** | CD pipeline: deploy to staging/production on tag or manual trigger. |
| `Dockerfile` | **CREATE** | Docker container for MVP (if containerization desired). |
| `docker-compose.yml` | **CREATE** | Local dev environment with SQL Server container. |
| `.env.example` | **CREATE** | Template for environment variables (database connection, API keys, etc.). |

#### Code Quality
| File | Status | Changes |
|------|--------|---------|
| `.editorconfig` | **CREATE** | EditorConfig file to enforce consistent code style (indentation, line endings, etc.). |
| `.gitignore` | **UPDATE** | Ensure all build artifacts, secrets, IDE files ignored. |
| `ArenaSync.Web.Tests/` | **COMPLETE** | All unit and integration tests created and passing. Minimum 80% code coverage. |

#### Refactoring & Cleanup
| File | Status | Changes |
|------|--------|---------|
| All `.cs` files | **REVIEW** | Code review for: DRY principle, naming conventions, error handling, logging. Refactor duplicates. |
| All `.razor` files | **REVIEW** | UI review for consistency, responsive design, accessibility. |
| `ArenaSync.Web/wwwroot/` | **CLEANUP** | Remove placeholder files, bundle/minify CSS/JS if needed. |

#### QA & Bug Fixes
| File | Status | Changes |
|------|--------|---------|
| Various | **ITERATION** | Conduct manual QA testing of all features. Log bugs. Fix critical/major bugs. Defer non-critical issues. |

### Deliverables
- ✅ Professional, responsive UI
- ✅ Complete documentation (README, architecture, dev guide, user guide, deployment)
- ✅ E2E test scripts ready for QA
- ✅ CI/CD pipelines configured
- ✅ Production configuration ready
- ✅ Code coverage 80%+
- ✅ MVP deployed to test/staging server
- ✅ Ready for stakeholder demo

---

## Summary Table

| Batch | Focus | Key Files/Folders | Timeline | Dependencies |
|-------|-------|-------------------|----------|--------------|
| 1 | Event & Venue CRUD | Models/, Services/, Pages/Events/*, Pages/Venues/*, Dtos/ | Week 1-2 | None |
| 2 | Teams & Attendees | Models/, Services/, Pages/Teams/*, Pages/Attendees/*, Dtos/ | Week 3-4 | Batch 1 |
| 3 | Vendors & Facilities | Models/, Services/, Pages/Vendors/*, Pages/Assignments/*, Dtos/ | Week 5-6 | Batch 1 |
| 4 | Reports & Dashboard | Services/ReportingService.cs, Pages/Dashboard.razor, Pages/Reports/* | Week 7 | Batches 1-3 |
| 5 | Auth & Validation | Program.cs, Data/, Pages/Auth/*, Models/, Middleware/ | Week 8 | Batches 1-4 |
| 6 | Polish & Docs | wwwroot/, Components/Layout/, *.md files, .github/workflows/ | Week 9 | Batches 1-5 |

---

## Critical Path

1. **Batch 1** establishes foundation (models, basic CRUD, services)
2. **Batches 2-3** expand domain (teams, attendees, vendors, assignments) — can be partially parallelized
3. **Batch 4** adds visibility (reports, dashboards)
4. **Batch 5** hardens system (auth, validation, error handling)
5. **Batch 6** final polish for release

Each batch is independently deployable; earlier batches provide value even if later batches are not completed.

---

**MVP Ready for Launch after Batch 6 ✅**
