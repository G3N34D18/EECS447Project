# ArenaSync — Testing Guide

## Automated Tests

Run the full test suite:

```bash
dotnet test
```

Expected: **73 tests, 0 failures**.

Run with detailed per-test output:

```bash
dotnet test --logger "console;verbosity=detailed"
```

Tests are organized as:

| Suite | Location | Count |
|---|---|---|
| ValidationService unit tests | Tests/Services/ValidationServiceTests.cs | 14 |
| Authentication integration tests | Tests/Integration/AuthenticationTests.cs | 10 |
| Validation integration tests | Tests/Integration/ValidationTests.cs | 9 |
| Assignment service tests | Tests/Services/AssignmentServiceTests.cs | varies |
| Facility service tests | Tests/Services/FacilityServiceTests.cs | varies |
| Reporting service tests | Tests/Services/ReportingServiceTests.cs | varies |
| Team service tests | Tests/Services/TeamServiceTests.cs | varies |
| Vendor service tests | Tests/Services/VendorServiceTests.cs | varies |

---

## Manual E2E Test Scenarios

See [`ArenaSync.Web.Tests/E2E/E2EScenarios.md`](ArenaSync.Web.Tests/E2E/E2EScenarios.md) for the full step-by-step manual QA scripts.

### Quick Smoke Test (5 minutes)

Run this after every deployment to verify the app is healthy:

1. Navigate to the app URL — expect the login page or home page
2. Sign in as `admin@arenasync.com` / `Admin123!` — expect redirect to home
3. Navigate to Events — expect the events list to load with seeded data
4. Navigate to Teams — expect the teams list to load
5. Navigate to Reports → Event Summary — expect a report to render
6. Sign out — expect redirect to login

### Full QA Checklist

Covered in E2EScenarios.md:

- [ ] TS-01: User Authentication
- [ ] TS-02: Venue Management
- [ ] TS-03: Event Management
- [ ] TS-04: Team Management
- [ ] TS-05: Attendee Management
- [ ] TS-06: Vendor Management
- [ ] TS-07: Team-Event Assignment
- [ ] TS-08: Vendor-Event Assignment
- [ ] TS-09: Attendee Registration
- [ ] TS-10: Reports
- [ ] TS-11: Role-Based Access Control
- [ ] TS-12: Validation & Error Handling
