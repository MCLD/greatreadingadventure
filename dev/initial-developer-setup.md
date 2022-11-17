# GRA Developer Documentation - Initial developer setup

## Development environment

- Windows, macOS: [Visual Studio 2022](https://www.visualstudio.com/vs/)
- Linux, macOS, Windows: [Visual Studio Code](https://code.visualstudio.com/)
  - Ensure you [install the C# extension](https://code.visualstudio.com/docs/runtimes/dotnet)

## Initial developer setup

The project ships with the Microsoft SQL Server data provider configured. If you are running in a Windows environment this will automatically use a [LocalDB](https://msdn.microsoft.com/en-us/library/hh510202.aspx) instance under `(localdb)\mssqllocaldb`. **In a Linux/macOS environment you should switch to the SQLite provider (in `appsettings.json` change the `GraConnectionStringName` to "SQLite"). You will need to create one master migration, the SQLite provider does not support all of the operations necessary to support incremental migrations.**

### Database migration

Database migrations are included with feature commits so in the case of using SQL Server you shouldn't need to add a migration to start using the software. If you wish to use SQLite or make changes to the data model, you must add database migrations to the project. The GRA applies all pending database migrations upon startup.

Migrations can be managed with the [`dotnet ef`](https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dotnet) command line tool or the [Package Manager Console](https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/powershell).

Connection strings for creating migrations are located in the `GRA.Development` namespace and mirror the definitions in the default `appsettings.json` file.

#### Using dotnet ef

1. Navigate to the appropriate project directory for your database provider (e.g. `src/GRA.Data.SqlServer` or `src/GRA.Data.SQLite`).
2. See a list of existing migrations:

   `dotnet ef -s ../GRA.Web migrations list`

3. Add a new migration for development:

   `dotnet ef -s ../GRA.Web migrations add develop`

4. Create or update the database to the migration (necessary for SQLite, possibly not for SQL Server but won't hurt):

   `dotnet ef -s ../GRA.Web database update`

#### Using the Package Manager Console

1. Choose the appropriate project from the "Default project" drop-down (e.g. `src/GRA.Data.SqlServer` or `src/GRA.Data.SQLite`).
2. Check if a database migration exists by looking for a folder named "Migrations" in the appropriate GRA.Data project.
3. Add a new migration for development:

   `Add-Migration develop`

4. Create or update the database to the migration (necessary for SQLite, possibly not for SQL Server but won't hurt):

   `Update-Database`

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

## Adding avatars

Avatars do not ship in the development tree, they live in a separate project on GitHub: [MCLD/gra-avatars](https://github.com/MCLD/gra-avatars). If you wish to import them, create a directory in the `shared` directory called `private` and place the latest avatar release ZIP there. With that file in place, the avatar import button will be displayed in Mission Control (/MissionControl/Avatars).
