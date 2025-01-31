# JustTip - Employee Tip Management System

JustTip is a web application for managing employee tips and work shifts in the service industry. It allows businesses to track employee shifts and distribute tips fairly based on worked days.


## Local Development Setup

### Prerequisites

- .NET 8.0 SDK
- SQL Server (LocalDB or higher)
- Visual Studio 2022 or VS Code
- Entity Framework Core tools (if not installed, run: `dotnet tool install --global dotnet-ef`)

### Setup Steps

1. Clone the repository:
```bash
git clone https://github.com/tomicl1/JustTip
cd justtip
```

2. Update the connection string in `appsettings.json` if needed (default uses LocalDB)

3. Apply database migrations:

   **Using .NET CLI in terminal:**
   ```bash
   dotnet ef database update
   ```

   > **Important**: Migrations must be applied before running the application. The build process alone won't create the database schema.

4. Run the seed data script located in `setup/seed-data.sql` to populate test data:
   - Open SQL Server Management Studio
   - Connect to your database
   - Open and execute `setup/seed-data.sql`

5. Build and run the application:
```bash
dotnet build
dotnet run --project src/JustTip.Web
```

The application should now be running at `https://localhost:5032` (or any other port that opens for you)


### Example Tip Distribution Scenarios

Here are real examples from our seed data showing how tips are distributed across different periods:

1. **Restaurant A - (2/1/2025 - 2/3/2025)**
   - Total Tips: $100.00
   - Total Working Days: 0 days across 3 employees
   - Distribution:
     No distribution

2. **Restaurant A - (2/4/2025 - 2/16/2025)**
   - Total Tips: $1,750.00
   - Total Working Days: 17 days across 2 employees
   - Distribution:
     * John Smith (8 days): $823.53
     * Jane Doe (9 days): $926.47

3. **Restaurant A - (2/18/2025 - 2/28/2025)**
   - Total Tips: $512.00
   - Total Working Days: 16 days across 3 employees
   - Distribution:
     * John Smith (7 days): $271.25
     * Jane Doe (2 days): $77.50
     * Bob Wilson (7 days): $271.25

4. **Cafe B- (2/1/2025 - 3/31/2025)**
   - Total Tips: $3,427.40
   - Total Working Days: 50 days across 2 employees
   - Distribution:
     * John Smith (19 days): $1302.41
     * Jane Doe (31 days): $2124.99

5. **Bar C- (12/1/2024 - 3/11/2025)**
   - Total Tips: $2,150.00
   - Total Working Days: 17 days across 1 employees
   - Distribution:
     * Frank Miller (17 days): $2150.00

## Running Tests

Execute the test suite using:
```bash
cd src/JustTip.Tests
dotnet test
```

The test project includes unit tests for:
- Controllers
- Repositories
- Business logic
- Edge cases in tip calculations 
