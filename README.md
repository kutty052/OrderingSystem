# OrderingSystem (.NET 7) - Starter Solution

This is a starter ASP.NET Core MVC project implementing a small Ordering System + Sales Dashboard using ADO.NET (no ORM).

## What's included
- ASP.NET Core MVC project scaffold
- ADO.NET `DbHelper` wrapper
- Controllers: Products, Orders, Dashboard, Account (cookie auth)
- Razor views for basic flows (Products list/create, Orders create/details, Dashboard)
- SQL scripts: `SqlScripts/schema.sql` and `SqlScripts/seeds.sql`
- README with setup steps

## Setup
1. Install .NET 7 SDK (https://dotnet.microsoft.com).
2. Create a SQL Server database (or use LocalDB). Update connection string in `appsettings.json`:
   ```
   "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER;Database=OrderingSystemDb;Trusted_Connection=True;"
   }
   ```
3. Run the SQL scripts in order:
   - `SqlScripts/schema.sql`
   - `SqlScripts/seeds.sql`
   Update the Users table to include a proper SHA256 base64 password hash for 'Password123' or create a user manually.

   Example to compute a SHA256 base64 in PowerShell:
   ```
   $bytes = [System.Text.Encoding]::UTF8.GetBytes("Password123")
   $hash = [System.Convert]::ToBase64String((New-Object System.Security.Cryptography.SHA256Managed).ComputeHash($bytes))
   ```

   Then update:
   ```
   UPDATE Users SET PasswordHash = '<base64-hash>' WHERE Username='admin';
   ```

4. From the project folder:
   ```
   dotnet restore
   dotnet run
   ```
5. Open `https://localhost:5001` (or the URL printed). Login at `/Account/Login` with `admin` and the password you set.

## Features
- CRUD for Products (AJAX-based search + pagination).
- Customers and Orders tables with relationships.
- Dashboard with KPIs and charts (sales trend, top products).
- Stored procedures replaced by repository (ADO.NET).
- Bootstrap 5 + jQuery UI.

## Tradeoffs
- Used **ADO.NET Repository Pattern** (lighter than EF Core, closer to assignment).
- Used **AJAX partials** for better UX but kept simple.
- For production, would add stored procs and stronger validation.


