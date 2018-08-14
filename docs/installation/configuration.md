# Configuration

Initial configuration for version 4 of The Great Reading Adventure involves setting up database connection information and the initial signup authorization code.

You can edit the `appsettings.json` file that comes with The Great Reading Adventure version 4 or you can create a new `appsettings.json` file that overrides the built-in application settings and place it in the `shared` directory under your GRA installation.

The critical settings to update are the "SqlServer" setting under "ConnectionStrings" and the "GraInitialAuthCode" setting.

**Please note:** Application settings are configured in a JSON or "JavaScript Object Notation" file. This file can be edited with any text editor (such as notepad.exe) but must be in a specific format. You can find validators online which will help you ensure that the syntax of the file is correct. Also note that when a backslash (`\`) or double quote (`"`) appears within quotes (for example in the database password) it must be escaped, meaning a backslash should appear prior to the escaped character (e.g. `\\` or `\"`).

## Connection string

```json
"ConnectionStrings": {
  "SqlServer": "Server=<servername>;Database=<databasename>;user id=<username>;password=<password>;MultipleActiveResultSets=true"
},
```

In the above example you'd replace the following:

- `<servername>` - Hostname or IP address of your SQL Server
- `<databasename>` - Name of the SQL Server database
- `<username>` - SQL Server login to use
- `<password>` - SQL Server password to use

For password generation, please consider using a utility like [pwgen](https://github.com/tytso/pwgen) in a Linux environment or something similar to the [online Diceware password generator](https://www.rempe.us/diceware/#eff).

## Authorization code

```json
  "GraInitialAuthCode": "<authorizationcode>"
```

This is the code that you will use when you set up your account to grant you full access to Mission Control (the adminsitrative interface) of the software. Please change this value to ensure that people who come across your site cannot grant themselves full adminsitrator access!

### Sample configuration override file

If you wish to override the `appsettings.json` file rather than edit the file that comes packaged with the GRA, you'll make a file called `appsettings.json` in the `shared` directory under your GRA installation. The contents of that file should be similiar to this (see above for how to change the configuration values):

```json
{
  "ConnectionStrings": {
    "SqlServer": "Server=<servername>;Database=<databasename>;user id=<username>;password=<password>;MultipleActiveResultSets=true"
  },
  "GraInitialAuthCode": "<authorizationcode>"
}
```
