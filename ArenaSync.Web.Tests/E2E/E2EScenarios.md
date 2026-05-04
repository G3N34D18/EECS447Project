# ArenaSync — End-to-End Test Scenarios

**Prerequisites:** App running at `https://localhost:5119`, database seeded with demo data.

---

## TS-01: User Authentication

### TS-01-A: Successful Login

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to `https://localhost:5119` | Redirected to `/auth/login` |
| 2 | Enter email: `admin@arenasync.com` | Field accepts input |
| 3 | Enter password: `Admin123!` | Field accepts input (masked) |
| 4 | Click **Sign in** | Redirected to home page, nav shows user email |

### TS-01-B: Invalid Credentials

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to `/auth/login` | Login page shown |
| 2 | Enter email: `admin@arenasync.com`, password: `wrongpassword` | — |
| 3 | Click **Sign in** | Error: "Invalid email or password. Please try again." |

### TS-01-C: Empty Fields Validation

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to `/auth/login` | Login page shown |
| 2 | Leave both fields blank and click **Sign in** | Validation errors: "Email is required." and "Password is required." |

### TS-01-D: Account Lockout

| Step | Action | Expected Result |
|---|---|---|
| 1 | Attempt login with wrong password 5 times | After 5th attempt: "Your account has been locked due to too many failed attempts." |

### TS-01-E: Sign Out

| Step | Action | Expected Result |
|---|---|---|
| 1 | Sign in as any user | Authenticated |
| 2 | Click **Sign out** in the nav | Redirected to `/auth/login`, session cleared |

---

## TS-02: Venue Management

### TS-02-A: Create Venue

| Step | Action | Expected Result |
|---|---|---|
| 1 | Sign in as Admin or Manager | Authenticated |
| 2 | Navigate to **Venues** | Venue list shown |
| 3 | Click **Add Venue** | Create form shown |
| 4 | Enter Name: `Test Arena`, Address: `123 Main St`, Capacity: `5000` | Fields accept input |
| 5 | Click **Save** | Redirected to venue list, `Test Arena` appears |

### TS-02-B: Edit Venue

| Step | Action | Expected Result |
|---|---|---|
| 1 | Click **Edit** on any venue | Edit form pre-populated with existing data |
| 2 | Change capacity to `9999` | Field updated |
| 3 | Click **Save Changes** | Redirected to list, capacity shows `9999` |

### TS-02-C: Delete Venue

| Step | Action | Expected Result |
|---|---|---|
| 1 | Click **Delete** on `Test Arena` | Confirmation page shown with venue details |
| 2 | Click **Confirm Delete** | Redirected to list, `Test Arena` removed |

### TS-02-D: Venue Validation

| Step | Action | Expected Result |
|---|---|---|
| 1 | Click **Add Venue**, leave Name blank | — |
| 2 | Click **Save** | Validation error: "Name is required." |

---

## TS-03: Event Management

### TS-03-A: Create Event

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to **Events** → **Create Event** | Create form shown |
| 2 | Enter Name: `Test Tournament` | — |
| 3 | Select a venue | — |
| 4 | Set Start Date to tomorrow, Start Time: 9:00 AM | — |
| 5 | Set End Date to tomorrow, End Time: 5:00 PM | — |
| 6 | Click **Create Event** | Redirected to event list, `Test Tournament` appears |

### TS-03-B: Edit Event

| Step | Action | Expected Result |
|---|---|---|
| 1 | Click **Edit** on `Test Tournament` | Form pre-filled with existing values |
| 2 | Change the name to `Test Tournament Updated` | — |
| 3 | Click **Save Changes** | Redirected to list, updated name shown |

### TS-03-C: Delete Event

| Step | Action | Expected Result |
|---|---|---|
| 1 | Click **Delete** on an event that has team/attendee assignments | Confirmation page |
| 2 | Confirm deletion | Event and all related assignments deleted, redirected to list |

### TS-03-D: End Before Start Validation

| Step | Action | Expected Result |
|---|---|---|
| 1 | Create event with End Time before Start Time | — |
| 2 | Submit | Error: "End time must be after start time." |

---

## TS-04: Team Management

### TS-04-A: Create Team

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to **Teams** → **Create Team** | Create form shown |
| 2 | Fill in all fields with valid data | — |
| 3 | Click **Save** | Redirected to team list, new team appears |

### TS-04-B: Edit Team

| Step | Action | Expected Result |
|---|---|---|
| 1 | Click **Edit** on any team | Edit form pre-filled |
| 2 | Change the manager name | — |
| 3 | Click **Save Changes** | Changes saved and reflected in list |

### TS-04-C: Delete Team

| Step | Action | Expected Result |
|---|---|---|
| 1 | Click **Delete** on a team | Confirmation shown |
| 2 | Confirm | Team removed from list |

---

## TS-05: Attendee Management

### TS-05-A: Create Attendee

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to **Attendees** → **Add Attendee** | Create form |
| 2 | Enter Name, Email, Phone | — |
| 3 | Click **Save** | Attendee appears in list |

### TS-05-B: Register Attendee for Event

| Step | Action | Expected Result |
|---|---|---|
| 1 | Click on an attendee to view details | Attendee detail page |
| 2 | Click **Register for Event** | Registration form |
| 3 | Select an event | — |
| 4 | Click **Register** | Registration confirmed |

### TS-05-C: Duplicate Registration Blocked

| Step | Action | Expected Result |
|---|---|---|
| 1 | Attempt to register the same attendee for the same event twice | Error: "This attendee is already registered for the selected event." |

---

## TS-06: Vendor Management

### TS-06-A: Create Vendor

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to **Vendors** → **Add Vendor** | Create form |
| 2 | Fill in Name, Type, Location, Phone | — |
| 3 | Click **Save** | Vendor appears in list |

### TS-06-B: Edit and Delete Vendor

| Step | Action | Expected Result |
|---|---|---|
| 1 | Edit a vendor's phone number | Changes save successfully |
| 2 | Delete a vendor | Vendor removed from list |

---

## TS-07: Team-Event Assignment

### TS-07-A: Assign Team to Event

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to **Events**, click **Assign Teams** on an event | Assignment manager |
| 2 | Select a team and a locker room at the event's venue | — |
| 3 | Click **Assign** | Assignment appears in the list |

### TS-07-B: Duplicate Assignment Blocked

| Step | Action | Expected Result |
|---|---|---|
| 1 | Attempt to assign the same team to the same event twice | Error: "This team is already assigned to this event." |

### TS-07-C: Locker Room Conflict Blocked

| Step | Action | Expected Result |
|---|---|---|
| 1 | Attempt to assign two teams to the same locker room for the same event | Error: "This locker room is already assigned to another team for this event." |

---

## TS-08: Vendor-Event Assignment

### TS-08-A: Assign Vendor to Event

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to **Events**, click **Vendor Assignments** | Assignment manager |
| 2 | Select a vendor and a booth | — |
| 3 | Click **Assign** | Assignment confirmed |

### TS-08-B: Booth Conflict Blocked

| Step | Action | Expected Result |
|---|---|---|
| 1 | Attempt to assign two vendors to the same booth for the same event | Error shown, assignment blocked |

---

## TS-09: Attendee Registration via Events Page

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to **Events**, click **Registrations** on an event | Registration list |
| 2 | Add an attendee | Attendee added to the list |
| 3 | Remove the same attendee | Attendee removed |

---

## TS-10: Reports

### TS-10-A: Event Summary Report

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to **Reports** → **Event Summary** | Report page loads |
| 2 | Select an event from the dropdown | Summary shows teams, vendors, and attendees for that event |

### TS-10-B: Conflict Report

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to **Reports** → **Conflict Report** | Report loads |
| 2 | Review output | Any overlapping venue bookings or capacity issues are listed |

---

## TS-11: Role-Based Access Control

### TS-11-A: Viewer Cannot Edit

| Step | Action | Expected Result |
|---|---|---|
| 1 | Sign in as `viewer@arenasync.com` / `Viewer123!` | Authenticated |
| 2 | Navigate to an Edit page directly (e.g., `/venues/edit/1`) | Redirected to `/auth/access-denied` |

### TS-11-B: Admin-Only Registration Page

| Step | Action | Expected Result |
|---|---|---|
| 1 | Sign in as Manager | Authenticated |
| 2 | Navigate to `/auth/register` | Redirected to `/auth/access-denied` |
| 3 | Sign in as Admin and navigate to `/auth/register` | Registration form shown |

---

## TS-12: Validation & Error Handling

### TS-12-A: Required Fields

| Step | Action | Expected Result |
|---|---|---|
| 1 | Open any Create form and submit with all fields blank | All required fields show validation errors |

### TS-12-B: Invalid Email

| Step | Action | Expected Result |
|---|---|---|
| 1 | Enter `notanemail` in any email field and submit | Validation error: "Please enter a valid email address." |

### TS-12-C: 404 Page

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to `/venues/edit/99999` (non-existent ID) | "Venue not found" message shown |
