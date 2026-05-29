# FysioFunc - Receptionssystem

FysioFunc is an internal booking and management system for BookRight Klinik og Wellness ApS, built for receptionists to manage clients, sessions, and promotions across multiple clinic locations.

---

## Getting Started

### Prerequisites

- .NET 10 SDK
- SQL Server (remote or local)
- Visual Studio 2022 or later (Ideally 2026)

### NuGet Packages
Core & Architecture
- **FluentResults** - Handles operation outcomes and business errors gracefully without throwing expensive exceptions.
- **ClosedXML** - Used to generate and format the multi-sheet Excel reports (.xlsx) for clinic statistics.
- **Microsoft.Extensions.DependencyInjection** - Handles Dependency Inversion (DI)

Data Access (Infrastructure)
- **Microsoft.EntityFrameworkCore** - The Object-Relational Mapper (ORM) used to bridge the domain with the database.
- **Microsoft.EntityFrameworkCore.SqlServer** - Database provider to connect EF Core specifically with SQL Server / SSMS.
- **Microsoft.EntityFrameworkCore.Tools** - Enables database migrations and management via the Package Manager Console.

FrontEnd (Presentation)
- **Radzen.Blazor** - UI component library used for it's easy access to ICONs

Testing
- **Moq** - Mocking framework used to isolate layers by simulating database repositories and dependencies during testing.
- **xunit** - The core testing framework used to structure and run our unit tests.
- **xunit.runner.visualstudio** - Integrates the xUnit test suite directly into Visual Studio's Test Explorer.

### Configuration

Open `appsettings.json` in `FysioEnterprise.Presentation` and ensure connectionstring is this one:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=root.molberg.cloud,6767;Initial Catalog=FysioFunc;Persist Security Info=False;User ID=sa;Password=Herax420!!;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Command Timeout=0"
  }
}
```

### Running the Application

1. Clone the repository
2. Set `FysioEnterprise.Presentation` as the startup project
3. Run the application - the database is created and seeded automatically on first launch

---

## Login

On the home screen, select a clinic and a receptionist to begin. No password is required - this is an internal tool intended for use within the clinic.

**Recommended clinic for demonstration:** `Vejle Klinik, Boulevarden 24, 7100 Vejle`

This clinic has the most seed data including overlapping sessions, multiple staff members, completed sessions with discounts applied, and a cancelled and no-show session for statistics demonstration.
Any receptionist is fine, only intended to demonstrate log-in.

---

## Features

### Booking Calendar (`/bookings`)
- Weekly, monthly and daily calendar views
- Sessions displayed with colour coding per staff member
- Opening hours visualised - closed hours shown with a blue overlay
- Click a row to highlight it, double-click a cell to create a new booking at that time
- Search sessions by staff member or client
- Toggle between staff and client calendar view
- Sessions can be marked as Completed, No-show or Cancelled directly from the calendar

### Sessions (`/createsessions`)
- Step-by-step booking form - client, time, session type, staff, room, promotion
- Combo booking - create two back-to-back sessions in a single flow. The second session pre-fills with the same client and starts immediately after the first. Both are submitted together.
- Session types are validated against combo booking rules when in combo mode
- Opening hours validation prevents bookings outside clinic hours

### Clients (`/clients`)
- Full client list with search by name
- Create, edit and delete clients
- Assign preferred staff member per client
- Loyalty level (None, Bronze, Silver, Gold) assigned automatically based on spend over the last 12 months

### Promotions (`/promotions`)
- Full promotion list with search by name 
- Create, edit and delete promotions with a discount percentage and date range
- Promotions are applied during session creation if desired and falls within the promotion period

### Statistics (`/statistics`)
- Revenue and session status graphs by week, month or year
- Navigate between periods using the date controls
- **Excel export** - select a date range and download a `.xlsx` report with three sheets:
  - **Revenue (Omsaetning)** - per-session revenue breakdown including original price, discount, lost revenue (cancelled/no-show) and upcoming revenue (active)
  - **Booking Status** - count and percentage of each session status
  - **Discounts (Rabatter)** - discount analysis showing which pricing strategy was applied per session

---

## Architecture

The solution follows Clean Architecture with four main layers:
- FysioEnterprise.Domain          - Entities, value objects, domain exceptions, pricingfactory
- FysioEnterprise.Facade          - DTOs, use case interfaces, query interfaces
- FysioEnterprise.UseCase         - Commandhandlers
- FysioEnterprise.Infrastructure  - EF Core, repositories, database, seed data
- FysioEnterprise.Presentation    - Blazor Server frontend
- FysioEnterprise.Testing         - Unit tests, MOQ-tests

## Testing

Unit tests are located in `FysioEnterprise.Testing`. Tests cover:

- Domain entity creation and validation
- Pricing strategy calculations
- Query handler logic using an in-memory database
- Opening hours validation

Run tests from Visual Studio Test Explorer or:

```powershell
dotnet test
```