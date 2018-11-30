# GRA Developer Documentation - Initial developer setup

## Development environment

- Windows: [Visual Studio 2017](https://www.visualstudio.com/vs/)
- Linux, macOS, Windows: [Visual Studio Code](https://code.visualstudio.com/)
  - Ensure you [install the C# extension](https://code.visualstudio.com/docs/runtimes/dotnet)

## Initial developer setup

#### *Due to changes in the data model during development, the project only ships with database migrations in place for releases. You will need to run the command below if there have been any database changes in the `develop` branch since the last release.*

The project ships with the Microsoft SQL Server data provider configured. If you are running in a Windows environment this will automatically use a [LocalDB](https://msdn.microsoft.com/en-us/library/hh510202.aspx) instance under `(localdb)\mssqllocaldb`. **In a Linux/macOS environment you should switch to the SQLite provider (in `appsettings.json` change the `GraConnectionStringName` to "SQLite").**

### Database migration

Initial database setup and configuration can be done utilizing the [`dotnet ef`](https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dotnet) command line tool or the [Package Manager Console](https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/powershell).

Connection strings for creating migrations are located in the `GRA.Development` namespace and mirror the definitions in the default `appsettings.json` file.

Here are steps for using `dotnet ef`:

  1. Navigate to the appropriate project directory for your database provider (e.g. `src/GRA.Data.SqlServer` or `src/GRA.Data.SQLite`).
  2. See a list of existing migrations:

    dotnet ef -s ../GRA.Web migrations list

  3. Add a new migration for development:

    dotnet ef -s ../GRA.Web migrations add develop

  4. Create or update the database to the migration (necessary for SQLite, possibly not for SQL Server but won't hurt):

    dotnet ef -s ../GRA.Web database update

Here are steps for using the Package Manager Console:

  1. Choose the appropriate project from the "Default project" drop-down (e.g. `src/GRA.Data.SqlServer` or `src/GRA.Data.SQLite`).
  2. Check if a database migration exists by looking for a folder named "Migrations" in the appropriate GRA.Data project.
  3. Add a new migration for development:

      Add-Migration develop

  4. Create or update the database to the migration (necessary for SQLite, possibly not for SQL Server but won't hurt):

      Update-Database

Please remove any non-release migrations when contributing code back to the project.

### Configuration

During development, User Secrets are the easiest way to customize the configuration without the risk of committing sensitive data to a source control repository. One key setting you may want to configure is `GraEmailOverride`. You can do that by right-clicking on the "GRA.Web" project and choosing "Manage User Secrets." Configure the `secrets.json` like this:

```json
{
  "GraEmailOverride": "your@email.address"
}
```

This will ensure that no errant emails are sent out during development. There are other settings you may want to configure through this manner such as `GraDefaultOutgoingMailHost` and `GraDefaultOutgoingMailPort`. Review [the documentation](http://manual.greatreadingadventure.com/en/latest/technical/appsettings/) for more configuration settings.

### Run the application!

At this point you should be able to run the application in the debugger.
