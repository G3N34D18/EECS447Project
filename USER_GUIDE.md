# ArenaSync — User Guide

## Getting Started

Navigate to the app URL and sign in with your credentials. Your role determines what you can access:

| Role | Access |
|---|---|
| Admin | Full access: create/edit/delete everything, manage users |
| Manager | Create/edit events, teams, vendors, venues; view reports |
| Viewer | Read-only access to all pages |

---

## Signing In

1. Go to `/auth/login`
2. Enter your email and password
3. Optionally check **Remember me** to stay signed in for 8 hours
4. Click **Sign in**

After 5 failed attempts your account is locked for 5 minutes.

---

## Venues

### Viewing Venues
Navigate to **Venues** in the sidebar. All venues are listed with their address and capacity.

### Creating a Venue
1. Click **Add Venue**
2. Fill in Name, Address, and Capacity
3. Click **Save** — the venue appears in the list immediately

### Editing a Venue
Click the **Edit** button next to any venue, update the fields, and click **Save Changes**.

### Deleting a Venue
Click **Delete** next to a venue and confirm. Note: venues with associated events cannot be deleted until those events are removed first.

### Managing Locker Rooms & Booths
From the venue list, click **Locker Rooms** or **Vendor Booths** to manage the physical spaces inside each venue.

---

## Events

### Viewing Events
Navigate to **Events** in the sidebar. Events are sorted by start time and show venue and timing info.

### Creating an Event
1. Click **Create Event**
2. Enter a name and description
3. Select the venue
4. Set the start and end date/time using the date picker and hour/minute/AM-PM selectors
5. Click **Create Event**

### Editing an Event
Click **Edit** on any event. All fields are editable. The system validates that end time is after start time and that no other event at the same venue overlaps.

### Deleting an Event
Click **Delete** and confirm. All associated team assignments, vendor assignments, and registrations are removed automatically.

### Assigning Teams to an Event
From the event list, click **Assign Teams** to open the team assignment manager for that event. Select a team and a locker room, then click **Assign**.

### Managing Vendor Assignments
Click **Vendor Assignments** from the event list to assign vendors to booths for that event.

### Managing Registrations
Click **Registrations** to see which attendees are registered and add or remove them.

---

## Teams

### Creating a Team
1. Go to **Teams** → **Create Team**
2. Fill in the team name, manager name, email, phone, and player count
3. Click **Save**

### Assigning a Team to an Event
From the team detail page, click **Assign to Event**. Select the event and a locker room at that event's venue.

### Requesting a Reassignment
If a team needs to move to a different locker room, use the **Request Reassignment** button. Admin users can approve or reject pending requests from the Admin panel.

---

## Attendees

### Creating an Attendee
1. Go to **Attendees** → **Add Attendee**
2. Enter name, email, and phone
3. Click **Save**

### Registering an Attendee for an Event
From the attendee detail page, click **Register for Event** and select an event. The system checks that the attendee is not already registered and that the venue has capacity.

---

## Vendors

### Creating a Vendor
1. Go to **Vendors** → **Add Vendor**
2. Fill in name, type, location, and phone
3. Click **Save**

### Assigning a Vendor to an Event
Vendor assignments are managed from the **Events** page using the **Vendor Assignments** button. Select the vendor and a booth at the event's venue.

---

## Reports

Navigate to **Reports** in the sidebar to access:

| Report | Description |
|---|---|
| Event Summary | Full roster of teams, vendors, and attendees per event |
| Team Schedule | All events a specific team is assigned to |
| Venue Events | All events scheduled at a specific venue |
| Conflict Report | Overlapping bookings and capacity issues |
| Export | Download report data |

---

## Admin Panel

Accessible to **Admin** users only via **Admin** in the sidebar.

- **Pending Requests**: View and approve/reject team reassignment requests
- **Register New User**: Create new user accounts and assign roles

---

## Signing Out

Click your email address in the top navigation bar and select **Sign out**, or navigate to `/auth/logout`.
