# GRA Developer Documentation - Initial developer setup

## Development environment

- Windows: [Visual Studio 2017](https://www.visualstudio.com/vs/)
- Linux, macOS, Windows: [Visual Studio Code](https://code.visualstudio.com/)
  - Ensure you [install the C# extension](https://code.visualstudio.com/docs/runtimes/dotnet)

*The project has been migrated to use Visual Studio 2017 `.csproj` files and no longer uses the `project.json` project files as in Visual Studio 2015.*

## Initial developer setup

#### *Due to changes in the data model during development, the project only ships with database migrations in place for releases. You will need to run the command below if there have been any database changes in the `develop` branch since the last release.*

The project ships with the Microsoft SQL Server data provider configured. If you are running in a Windows environment this will automatically use a [LocalDB](https://msdn.microsoft.com/en-us/library/hh510202.aspx) instance under `(localdb)\mssqllocaldb`. **In a Linux/macOS environment you should switch to the SQLite provider (in `appsettings.json` change the `GraConnectionStringName` to "SQLite").**

### Database migration

Initial database setup and configuration can be done utilizing the [`dotnet ef`](https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dotnet) command line tool or the [Package Manager Console](https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/powershell). Here are steps for using `dotnet ef`:

  1. Navigate to the appropriate project directory for your database provider (e.g. `src/GRA.Data.SqlServer` or `src/GRA.Data.SQLite`).
  2. Check if a database migration exists:

    dotnet ef -s ../GRA.Web migrations list

  3. If no migrations exist, create one:

    dotnet ef -s ../GRA.Web migrations add develop

  4. Create or update the database to the migration (necessary for SQLite, possibly not for SQL Server but won't hurt):

    dotnet ef -s ../GRA.Web database update

### Configuration

The `GRA.Web` project has the [Secret Manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets#secret-manager) enabled. You may want to issue a command such as:

    dotnet user-secrets set GraEmailOverride your@email.address

To ensure that no errant emails are sent out during development. There are other settings you may want to configure through this manner such as `GraDefaultOutgoingMailHost` and `GraDefaultOutgoingMailPort`. Review `Startup.cs` for more configuration settings.

### Run the application!

At this point you should be able to run the application in the debugger.
