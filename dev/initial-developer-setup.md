# GRA 4 Developer Documentation - Initial developer setup

## Development environment

- Windows: [Visual Studio](https://www.visualstudio.com/vs/)
- Linux, macOS, Windows: [Visual Studio Code](https://code.visualstudio.com/)
  - Ensure you [install the C# extension](https://code.visualstudio.com/docs/runtimes/dotnet)

## Database provider selection

Currently the application supports using Microsoft SQL Server and SQLite.

### SQL Server

Ensure the following lines are uncommented in the `GRA.Web/Startup.cs`

- `Configuration["ConnectionStrings:DefaultConnection"] = @"Server=(localdb)\mssqllocaldb;Database=gra4;Trusted_Connection=True;MultipleActiveResultSets=true";`
- `services.AddScoped<Data.Context, Data.SqlServer.SqlServerContext>();`

**Note: if you wish to change the development connection string you will also need to do it in `GRA.Data.SqlServer/SqlServerContext.cs`.**

### SQLite

Ensure the following lines are uncommented in the `GRA.Web/Startup.cs`

- `Configuration["ConnectionStrings:DefaultConnection"] = @"Filename=./gra4.db";`
- `services.AddScoped<Data.Context, Data.SQLite.SQLiteContext>();`

**Note: if you wish to change the development connection string you will also need to do it in `GRA.Data.SQLite/SQLiteContext.cs`.**

## Initial database setup

Initial database setup and configuration can be done utilizing the `dotnet` commandline tool.

1. Navigate to the appropriate project directory for your database provider (e.g. `GRA.Data.SqlServer` or `GRA.Data.SQLite`).
2. Set up the database by running: `dotnet ef --startup-project ../GRA.Web database update`
3. If there are no migrations (`dotnet ef --startup-project ../GRA.Web migrations list`):
  1. Create an initial migration: `dotnet ef --startup-project ../GRA.Web migrations add initial` (You don't have to name it "initial").
  2. Update the database again (`dotnet ef --startup-project ../GRA.Web database update`)

## Run the application

At this point you should be able to run the application in the debugger.
