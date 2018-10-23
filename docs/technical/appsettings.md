# Application Settings

ASP.NET Core can check several locations for configuration settings. It checks the following places in order:

1. `appsettings.json` in the deployed application directory (where the `GRA.dll` and `GRA.Web.dll` files are)
2. `shared/appsettings.json` in the deployed application directory - settings in this file override any settings in the top level `appsettings.json` file
3. Environment variables - any configured environment variables are passed into the software. If you don't wish to put sensitive information (such as your configuration string) into a file in the application directory you can [configure those items via environment settings](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-2.1#environment-variables-configuration-provider).

The majority of the software configuration occurs in the `appsettings.json` file which is a [JSON-formatted configuration file](https://json.org/example.html).

## Connection strings

One connection string is required (either `SqlServer` or `SQLite`).

- `SqlServer` - A SQL Server connection string
- `SQLite` - SQLite connection information (typically the path to the SQLite database file)
- `SqlServerSessions` - *optional* - A SQL Server connection string for [storing session data in a SQL Server database](https://docs.microsoft.com/en-us/aspnet/core/performance/caching/distributed?view=aspnetcore-2.1#using-a-sql-server-distributed-cache) (necessary for multiple Web servers answering requests for the same site)
- `SqlServerSerilog` - *optional* - A SQL Server connection string used for [storing SQL Server application logs](https://github.com/serilog/serilog-sinks-mssqlserver); the user should have database owner access (at least initially) so that it can create the proper table for logging

## General settings

- `GraConnectionStringName` - which connection string to use (either `SqlServer` or `SQLite`)
- `GraCulture` - *optional* - defaults to "en-US", the culture to use for displaying things like dates and times - for valid options see the language tags listed in the Microsoft [National Language Support (NLS) API Reference](http://go.microsoft.com/fwlink/?LinkId=200048)
- `GraInitialAuthCode` - the Authorization Code entered to grant users full access to the site - **it's important that you change this!**
- `GraInitialProgramSetup` - *optional* - defaults to "multiple" which creates four age-based programs and sets up a point translation of one minute read equals one point, can also be set to "single" which creates one program and sets up a point translation of one book read equals one point
- `GraMaximumAllowableActivity` (v4.1) - *optional* - for reporting purposes, limit participant activity above this amount to the "achiever" level configured for the participant's program
- `GraReverseProxyAddress` - *optional* - if provided, internally the software will disregard proxy IP addresses
- `GraRollingLogPath` - defaults to "shared/logs", a path to save a daily-rotating log file - if `GraInstanceName` is specified in `appsettings.json` it will be included in the log file name
- `GraSqlServer2008` - *optional* - if you are using SQL Server 2008, put text into this setting (any text will do)

## Default settings

These settings are used when the program runs for the first time to insert some reasonable defaults into the database. All of these settings are optional. All of these settings can be configured in the Site Settings area of Mission Control.

- `GraDefaultSiteName` - defaults to "The Great Reading Adventure", what the site refers to itself as
- `GraDefaultPageTitle` - defaults to "Great Reading Adventure", set in many page titles
- `GraDefaultSitePath` - defaults to "gra", this is used for tenancy (which is not implemented yet)
- `GraDefaultFooter` - the footer output on every web page
- `GraDefaultOutgoingMailHost` - the hostname or IP address of the outgoing mail server
- `GraDefaultOutgoingMailLogin` - login name for the mail server (if needed)
- `GraDefaultOutgoingMailPassword` - password for the mail server (if needed)
- `GraDefaultOutgoingMailPort` - defaults to "25", port to connect to for relaying SMTP emails

## Static file settings

- `GraContentDirectory` - defaults to "content/shared", the path to the shared content files for this instance of the application
- `GraContentPath` - defaults to "content", the URL path to the files in the `GraContentDirectory` (e.g. by default accessing /content/ with your Web browser serves files off the disk from the content/shared directory)

## Distributed cache and multiple front-end settings
- `GraApplicationDescriminator` - defaults to "gra", application discriminator to use for data protection
- `GraDataProtectionPath` - defaults to "shared/dataprotection", location to save the [data protection key](https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/?view=aspnetcore-2.1) if not using Redis as a distributed cache
- `GraDistributedCache` - *optional* - select a system to use for distributed cache: "Redis" or "SqlServer", anything else uses an in-memory distributed cache
- `GraInstanceName` - the name of this deployed instance
- `GraRedisConfiguration` - *optional* - address of a Redis server for distributed cache, only used if `GraDistributedCache` is set to "Redis"
- `GraSqlSessionSchemaName` - *optional* - the schema to use for the SQL Server distributed cache table, defaults to "dbo"
- `GraSqlSessionTable` - *optional* - the table to use for the SQL Server distributed cache, defaults to "Sessions"

## Logging with Serilog

Customization can be done to the way Serilog works by adding a "Serilog" section to the log file. For example, logging to Slack can be added by putting the following configuration section in (and replacing `<webhook URI>` with the [actual Slack incoming webhook URI](https://my.slack.com/services/new/incoming-webhook/)):

```json
"Serilog": {
  "MinimumLevel": "Debug",
  "Enrich": [ "FromLogContext" ],
  "WriteTo": [
    {
      "Name": "Slack",
      "Args": {
        "webhookUri": "<webhook URI>",
        "restrictedToMinimumLevel": "Warning"
      }
    }
  ]
}
```

More information about customizing Serilog in `appsettings.json` can be found in the [serilog-settings-configuration project on GitHub](https://github.com/serilog/serilog-settings-configuration).
