# GRA 4 Developer Documentation - Initial developer setup

## Development environment

- Windows: [Visual Studio](https://www.visualstudio.com/vs/)
- Linux, macOS, Windows: [Visual Studio Code](https://code.visualstudio.com/)
  - Ensure you [install the C# extension](https://code.visualstudio.com/docs/runtimes/dotnet)

## Initial developer setup

#### *Due to the constantly changing data model the project currently does not yet ship with a database migration in place. You must run the command below to set up an initial migration.*

The project ships with the Microsoft SQL Server data provider configured. If you are running in a Windows environment this will automatically use a [LocalDB](https://msdn.microsoft.com/en-us/library/hh510202.aspx) instance under `(localdb)\mssqllocaldb`. **In a Linux/macOS environment you should switch to the SQLite provider (see *Database provider selection*) below.**

### Database migration

Initial database setup and configuration can be done utilizing the `dotnet` command line tool.

  1. Navigate to the appropriate project directory for your database provider (e.g. `src/GRA.Data.SqlServer` or `src/GRA.Data.SQLite`).
  2. Check if a database migration exists:

    `dotnet ef --startup-project ../GRA.Web migrations list`

  3. If no migrations exist, create one:

    `dotnet ef --startup-project ../GRA.Web migrations add initial`

### Run the application!

At this point you should be able to run the application in the debugger.

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
