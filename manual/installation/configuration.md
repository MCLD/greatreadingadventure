# Configuration

Initial configuration for version 4 of The Great Reading Adventure involves setting up database connection information and the initial signup authorization code. There are additional configuration settings but they are optional.

To customize settings in The Great Reading Adventure, create a file named `appsettings.json` and place it in the `shared` directory under your GRA installation. Ideally customization happens in this directory so that changes are not overwritten when updating the site with newer versions of the software. For a starting point, the `appsettings.json` file can be copied from the installation directory into the `shared` directory to provide a starting point. Alternately, a new text file can bre created as long as the extension of the file is `.json` and not `.txt` (be aware that by some operating systems (like Windows Server) tend to hide file extensions by default).

The critical settings to provide are the "SqlServer" setting under "ConnectionStrings" and the "GraInitialAuthCode" setting. Most of the other settings [can be customized](../../setup/site-customizations) once you authenticate with an administrator-level account.

**Please note:** Application settings are configured in a [JSON](https://json.org/example.html) or "JavaScript Object Notation" file. This file can be edited with any text editor (such as notepad.exe) but must be in a specific format. You can find validators online which will help you ensure that the syntax of the file is correct. Also note that when a backslash (`\`) or double quote (`"`) appears within quotes (for example in the database password) it must be escaped, meaning a backslash should appear prior to the escaped character (e.g. `\\` or `\"`).

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

For password generation, please consider using a utility like [pwgen](https://github.com/tytso/pwgen) in a Linux environment or something similar to the [online Diceware password generator](https://www.rempe.us/diceware/#eff). If you can create a long and complex password without backslash (`\`) or double quote (`"`) in it you will not have to worry about escaping those characters in the configuration file.

## Authorization code

```json
  "GraInitialAuthCode": "<authorizationcode>"
```

This is the code that you will use when you set up your administrator account to grant you full access to Mission Control (the administrative interface) of the software. Please change this value to ensure that people who come across your site cannot grant themselves full administrator access!

### Sample configuration file

Here's what your `appsettings.json` file in your `shared` directory might look like if you are changing those two required configuration settings:

```json
{
  "ConnectionStrings": {
    "SqlServer": "Server=<servername>;Database=<databasename>;user id=<username>;password=<password>;MultipleActiveResultSets=true"
  },
  "GraInitialAuthCode": "<authorizationcode>"
}
```

## More configuration options

The [Application Settings](../../technical/appsettings) section of the manual provides a comprehensive list of settings that can be configured in the `appsettings.json` file.
