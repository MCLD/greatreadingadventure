# System Requirements

The GRA requires the following services to run. These services may all be hosted on the same machine
or may be hosted on separate machines for performance reasons.

## Web server
[Microsoft Internet Information Services (IIS)](https://www.iis.net/) version 7.0
  or later.
  * Windows Server 2008 shipped with IIS 7 making it the minimum requirement for operating system.
  * The [.NET Framework](https://msdn.microsoft.com/en-us/vstudio/aa496123.aspx) version 4.0 or later must be installed with IIS configured to run ASP.NET pages.
  * Ideally, the GRA runs in its own Application Pool configured in the **Integrated managed pipline** mode.
  * File management (i.e. uploading images, configuration changes, etc.) occur by allowing the account running the App Pool to have write access to the Web server files on disk. In many configurations, that means that the `IUSR` user should be granted write access to the file system.

## Database server
[Microsoft SQL Server](http://www.microsoft.com/en-us/server-cloud/products/sql-server/) 2008 or later.
  * The GRA uses its own database with its own database users so it can easily and safely coexist with other SQL Server databases.
  * SQL Server must allow connections via [SQL Server authentication mode](https://msdn.microsoft.com/en-us/library/ms144284.aspx).
  * The GRA requires an empty database to be created manually or with a script.
  * The GRA also requires two users for initial setup:
    * A database user with owner access (role `db_owner`) for creating the initial database structures and inserting the initial
      data.
    * A database user with read/write access (roles `db_datareader` and `db_datawriter`) to the database for running the GRA software.


## Mail server
A mail service accepting mail via SMTP.
  * The GRA sends mail in certain instances and requires the ability to connect to an SMTP server.
  * The SMTP server built-in to IIS can be used, any other on-site mail server can be used (Microsoft Exchange Server, Postfix, etc.), or a mail service can be used.

## Checklist

- [ ] Ensure you have a Windows server running Windows Server 2008 or newer
- [ ] Ensure your server has the .NET Framework version 4.0 or newer installed
- [ ] Confirm that you can create a new Web site in IIS on this server
- [ ] Confirm that you will be permitted to configure it so that Web site files can be writable by the `IUSR` user.

- [ ] Ensure you have access to a Microsoft SQL Server version 2008 or newer
- [ ] Ensure that you'll be able to authenticate in **SQL Server authentication mode**
- [ ] Confirm that you'll be able to create a database
- [ ] Ensure that you have a mail server with an accessible SMTP port
- [ ] Collect the hostname or IP address and any required authentication information for connecting to the mail server