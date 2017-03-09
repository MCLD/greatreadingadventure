# GRA 4 Developer Documentation - Initial developer setup

## Development environment

- Windows: [Visual Studio 2017](https://www.visualstudio.com/vs/)
- Linux, macOS, Windows: [Visual Studio Code](https://code.visualstudio.com/)
  - Ensure you [install the C# extension](https://code.visualstudio.com/docs/runtimes/dotnet)

*The project has been migrated to use Visual Studio 2017 `.csproj` files and no longer uses the `project.json` project files as in Visual Studio 2015.*

## Initial developer setup

#### *Due to the constantly changing data model the project currently does not yet ship with a database migration in place. You must run the command below to set up an initial migration.*

The project ships with the Microsoft SQL Server data provider configured. If you are running in a Windows environment this will automatically use a [LocalDB](https://msdn.microsoft.com/en-us/library/hh510202.aspx) instance under `(localdb)\mssqllocaldb`. **In a Linux/macOS environment you should switch to the SQLite provider (see *Database provider selection*) below.**

### Database migration

Initial database setup and configuration can be done utilizing the [`dotnet ef`](https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dotnet) command line tool or the [Package Manager Console](https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/powershell). Here are steps for using `dotnet ef`:

  1. Navigate to the appropriate project directory for your database provider (e.g. `src/GRA.Data.SqlServer` or `src/GRA.Data.SQLite`).
  2. Check if a database migration exists:

    `dotnet ef -s ../GRA.Web migrations list`

  3. If no migrations exist, create one:

    `dotnet ef -s ../GRA.Web migrations add initial`

  4. Create or update the database to the migration (necessary for SQLite, possibly not for SQL Server but won't hurt):

    `dotnet ef -s ../GRA.Web database update`

### Configuration

The `GRA.Web` project has the [Secret Manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets#secret-manager) enabled. You may want to issue a command such as:

```c#
  dotnet user-secrets set GraEmailOverride your@email.address
```

To ensure that no errant emails are sent out during development. There are other settings you may want to configure through this manner such as `GraDefaultOutgoingMailHost` and `GraDefaultOutgoingMailPort`. Review `Startup.cs` for more configuration settings.

### Run the application!

At this point you should be able to run the application in the debugger.

## Database provider selection

Currently the application supports using Microsoft SQL Server and SQLite. Default developer database connection strings are in the `GRA.DefaultConnectionString` namespace (in the `GRA` project).

### SQL Server

Ensure the following is the only uncommented line in the `GRA.Web/Startup.cs` under the `//database` comment.

- `services.AddScoped<Data.Context, Data.SqlServer.SqlServerContext>();`

### SQLite

Ensure the following is the only uncommented line in the `GRA.Web/Startup.cs` under the `//database` comment.

- `services.AddScoped<Data.Context, Data.SQLite.SQLiteContext>();`
